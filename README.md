# MulticlientServer
This is a simple chatting program written in C# with standard libraries, such as System.Net and System.Threading with the addition of logging. Made as a final project for my 4th semester.

So, basically, I had to make a program for my college and this is what I come up with. There were three major things that the program should do, which are:
1.  The program must be able to connect multiple clients with the server doing the broadcasting,
2.  The program must use different threads to handle each connected clients, and
3.  The program must be able to log the events and information that happened while the server is runnning.

Most of the examples I've seen in the internet were mostly local clients which connects to a local server. This program is fairly the same, it's just that it can connect devices with different ips instead of one ip with different ports. The references I used had client and server program separated, but to make it simple, I made it into one program.
