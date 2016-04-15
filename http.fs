#! /usr/local/bin/gforth
warnings off

include unix/socket.fs

8080 constant SERVER-PORT
255  constant QUEUE-SIZE
4096 constant MAX-SIZE
0    constant INIT

\ Almost exactly the same as scan, but excludes search character
: -scan ( addr u c -- addr' u' )
    scan 1- swap 1+ swap ;

: cut> ( addr u c -- addr u' )
    swap 0 DO 2dup swap I + c@ = IF
            drop I LEAVE THEN LOOP ;

: http/type ( -- addr u )
    s\" Content-Type: text/html\n\n" ;

: http-header ( addr u -- addr u )
    http/type s+ ;

: add-header
    http-header 2swap s+ ;

: http/ok ( -- addr u )
    s\" HTTP/1.1 200 OK\n" ;

: http/404 ( -- addr u )
    s\" HTTP/1.1 404 Not Found\n" ;

: /404 ( -- addr u )
    s" <html><body><center><h1>404 Not Found</h1></center></body></html>"
    http/404 add-header ;

: exists? ( addr u -- f )
    2dup file-status nip 0= ;

: safe? ( addr u -- f )
    2dup drop c@ [CHAR] / <> ;

: valid? ( addr u -- f )
    exists? >R safe? R> and ;

: send-file ( addr u -- addr' u' )
    slurp-file http/ok add-header ;

: route ( addr u -- addr2 u2 )
    valid? IF send-file ELSE
        2drop /404 THEN ;

: read-request ( socket -- addr u ) pad MAX-SIZE read-socket ;

: send-response ( addr u socket -- )
    dup >R write-socket R> close-socket ;

: GET ( addr u -- addr u )
    [CHAR] / -scan BL cut> route ;

: parse-request ( addr u -- )
    2dup BL cut> s" GET" str= IF GET ELSE 2drop /404 THEN ;

: http-server { server client }
    BEGIN
        server QUEUE-SIZE listen
        server accept-socket TO client

        client read-request parse-request client send-response
    AGAIN ;

SERVER-PORT create-server INIT http-server