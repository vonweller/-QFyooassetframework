set WORKSPACE=.
set LUBAN_DLL=%WORKSPACE%\Luban\Luban.dll
set CONF_ROOT=.

dotnet %LUBAN_DLL% ^
    -t all ^
    -c cs-simple-json ^
    -d json ^
    --conf %CONF_ROOT%\luban.conf ^
    -x outputCodeDir=..\Assets\QFramework\Toolkits\_LubanKit\cs^
    -x outputDataDir=..\Assets\QFramework\Toolkits\_LubanKit\jsonconfig
pause