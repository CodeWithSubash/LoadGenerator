PROBLEM:
As a company that runs HTTP services, questions of scale often come up. When you want to determine how a service will scale
before turning it loose in the wild, it's often prudent to run a load test to simulate your expected traffic. A good load 
generator should be able to provide ample RPS (requests per second) to help drive out potential performance problems before
any of your new users see them - the higher the better.

We would like you to construct a simple HTTP load generator client using modern C# async practices. 
1. It should accept an input file specifying details like hostname, path, and requests per second
2. Cotinuously generate the requested load until the program has been shut down.
3. Handle/report on any obviously erroneous behavior from the server.

This task should be timeboxed at somewhere around 3 hours; we are not expecting a world-class application, but merely
would like to get to know you better as a developer through your code. 

DETAILS:
   * Server URL: https://c1i55mxsd6.execute-api.us-west-2.amazonaws.com/Live
   * Permissions (in Header): 'X-Api-Key: RIqhxTAKNGaSw2waOY2CW3LhLny2EpI27i56VA6N'
   * Expected Request Payload (in JSON): { "name": "YOUR_NAME", "date": "NOW_IN_UTC", "requests_sent": REQUESTS_THIS_SESSION }
   * Expected Response Payload (in JSON): { "successful": true }

REQUIREMENTS:
   * Program must accept file-based input for: serverURL, targetRPS, authKey. Additional parameters may be added as desired for
     your clarity and ease of use.
   * Program must send up valid request body payload.
   * Program must sanely handle typical HTTP server responses.
   * Program must output to the console the current RPS and target RPS.
   * After the run has completed, program must output a summary of run including relevant request/response metrics.
   * Your API key is limited to 100,000 requests. Please contact us if you need that limit raised for any reason.

SUBMISSION:
We will review at the end of the interviewing session, and talk through design decisions and tradeoffs.


USAGE: 
[POST] https://c1i55mxsd6.execute-api.us-west-2.amazonaws.com/Live
[Authorization] Key: x-api-key; Value: RIqhxTAKNGaSw2waOY2CW3LhLny2EpI27i56VA6N
[Body] 
{
    "name": "Subash",
    "date": "2021-09-22",
    "Status": true
}


MY OBSERVATION (Using Postman):
1. Forbidden 
2. "message": "Unsupported HTTP Verb. Try POST."
3. "error": "json body is required"
4. "error": "'name' and 'date' are required fields"
5. "successful": true
6. "message": "Too Many requests"
7. "error": "Correctly formatted json body is required" [400 Bad Request]

DESIGN CONSIDERATIONS:
Create HttpClient object as a Singleton, as we don't need to create the instance for every request
Use DefaultRequestHeaders for headers that are common to all requests
Any other request-specific headers are put on HttpRequestMessage
If we are not wanting to use async, using .Result will force the code to execute synchronously

Using Log4Net DESIGN CHOICES:
Using Microsoft recommended one of the third-party logger frameworks, Log4Net
Using a simplest approach for logging, directly forwarding all logs to trusted log4net library
Static constructor will take care of configuration, no need to configure in every projects/library
All logging logic is encapsulated in one class, log4net can be easily replaced by any other library, if needed

OBSERVATIONS on RPS:
(1) 10-12 RPS (Very Low)
-- Remove the return content code
(2) TaskCanceledException (See exception1 below) [Set the higher timeout i.e. 30secs]
(3) 80 RPS (Implemented with Thread.Sleep(10) between each requests)
(4) 110+ RPS 99% success 110-120 RPS (Use Thread.Sleep(7))
(5) 140+ RPS 90% Success, 10% Failure (Use Thread.Sleep(5)) - See Exception2
(6) 140+ RPS 10% Success, 90% Failure (No Thread or, Thread.Sleep(1)) - See Exception2


OBSERVED EXCEPTIONS:
Exception1:
System.Threading.Tasks.TaskCanceledException: The operation was canceled.
 ---> System.IO.IOException: Unable to read data from the transport connection: The I/O operation has been aborted because of either a thread exit or an application request..
 ---> System.Net.Sockets.SocketException (995): The I/O operation has been aborted because of either a thread exit or an application request.
   --- End of inner exception stack trace ---

Exception2:
System.Net.Http.HttpRequestException: No connection could be made because the target machine actively refused it.
 ---> System.Net.Sockets.SocketException (10061): No connection could be made because the target machine actively refused it.
   at System.Net.Http.ConnectHelper.ConnectAsync(String host, Int32 port, CancellationToken cancellationToken)
   --- End of inner exception stack trace ---

NEWER APPROACH
On exception2, it is likely because the server has a full 'backlog'. So, we can perform some retry logic


