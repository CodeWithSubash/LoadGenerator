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