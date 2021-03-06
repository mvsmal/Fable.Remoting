namespace Fable.Remoting.Client 

open Fable.Import.Browser 

module Http = 

    /// Constructs default values for HttpRequest
    let private defaultRequestConfig : HttpRequest = {
        HttpMethod = HttpMethod.GET 
        Url = "/" 
        Headers = [ ]
        RequestBody = None
    }

    /// Creates a GET request to the specified url
    let get (url: string) : HttpRequest = 
        { defaultRequestConfig 
            with Url = url
                 HttpMethod = HttpMethod.GET }
    
    /// Creates a POST request to the specified url
    let post (url: string) : HttpRequest = 
        { defaultRequestConfig 
            with Url = url
                 HttpMethod = HttpMethod.POST }

    /// Creates a request using the given method and url
    let request method url = 
        { defaultRequestConfig
            with Url = url
                 HttpMethod = method }
    
    /// Appends a request with headers as key-value pairs
    let withHeaders headers (req: HttpRequest) = { req with Headers = headers  }
    
    /// Appends a request with string body content
    let withBody body (req: HttpRequest) = { req with RequestBody = Some body }

    /// Sends the request to the server and asynchronously returns a response
    let send (req: HttpRequest) =
        Async.FromContinuations <| fun (resolve, _, _) -> 
            let xhr = XMLHttpRequest.Create()
            
            match req.HttpMethod with 
            | HttpMethod.GET -> xhr.``open``("GET", req.Url)
            | HttpMethod.POST -> xhr.``open``("POST", req.Url)
                
            // set the headers, must be after opening the request
            for (key, value) in req.Headers do 
                xhr.setRequestHeader(key, value)

            xhr.onreadystatechange <- fun _ ->
                match xhr.readyState with
                | 4.0 (* DONE *) -> resolve { StatusCode = unbox xhr.status; ResponseBody = xhr.responseText }
                | otherwise -> ignore() 
        
            xhr.send(defaultArg req.RequestBody null)