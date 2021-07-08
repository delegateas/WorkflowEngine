
# Workflow Engine on top of Hangfire

```
docker run -e 'ACCEPT_EULA=Y' -e 'SA_PASSWORD=Bigs3cRet' -e 'MSSQL_PID=Express' -p 1434:1433 --name hangfiredemo -d mcr.microsoft.com/mssql/server:2017-latest-ubuntu
docker exec -it hangfiredemo /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P Bigs3cRet -Q "CREATE DATABASE hangfiredemo"
```

and run the demo app and go to /magic endpoint