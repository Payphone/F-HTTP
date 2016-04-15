# F-HTTP
A basic HTTP server written in Forth

## Introduction
This is a very basic HTTP server that can only read GET requests and respond
with a HTML file. It requires Gforth and uses the included socket library that
has been ported from C. The main goal behind this was getting used to string
manipulations in Forth.

## Usage
`` gforth http.fs ``