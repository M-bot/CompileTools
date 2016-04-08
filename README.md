# CompileTools
[![Build Status](https://travis-ci.org/M-bot/CompileTools.svg?branch=master)](https://travis-ci.org/M-bot/CompileTools)

CompileTools is a romhacking program being developed mainly by M-bot, blacksmithgu, and programmers at Heroes of Legend.

### Games
  * Nayuta no Kiseki
  * Wander Wonder
  * 46 Okunen

### Compression
  * CNX
  
### Archives
  * FLD
  * IT3 (WIP)
  * MLK (WIP)
  
### Formats
  * GDT (WIP)
  * GMP
  * ITV MMV3 1543 (Extract)
  * ITV MMV3 1286 (Planned)

### Commands
 * convert [file] [using]
 * compress [file] [using]
 * pack [file] [using]

### Examples
```
 convert file.gmp       -> file.bmp
 compress file.cnx      -> file.gmp
 compress file.gmp      -> file.cnx
 pack file.it3          -> file.it3_index + file (directory)
 pack file.it3_index    -> file.it3
 compress file.abc cnx  -> file.gmp
```
