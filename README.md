# Parser in Blazor Serve App

The main idea is to have a page with text file upload with capability (**files up to 3 MiB**)
App parses text file with the following structure:

```
Event-Name-1; Event-Description-1; Start-Date-Time-1; End-Date-Time-1;[EOL]
...
Event-Name-N; Event-Description-N; Start-Date-Time-N; End-Date-Time-N;[EOL]
```

The following requirements for input data validation should be satisfied:
* Event-Name: cannot be empty, up to 32 characters long
* Event-Description: cannot be empty, up to 255 characters long
* Start-Date date/time format: yyyy-MM-ddTHH:mmzzz
* End-Date date/time format: yyyy-MM-ddTHH:mmzzz
* App should be able to present the parsing result
* App should be able to report incorrect data in a file
