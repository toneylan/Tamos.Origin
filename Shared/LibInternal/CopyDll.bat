::用于Project中复制dll和xml文件
@echo off

set SrcFile=%1
set TgtDir=%2

if "%TgtDir%"=="" set TgtDir=%~dp0

if exist "%SrcFile%.dll" copy "%SrcFile%.dll" "%TgtDir%"
if exist "%SrcFile%.xml" copy "%SrcFile%.xml" "%TgtDir%"