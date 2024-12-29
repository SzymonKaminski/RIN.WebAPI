namespace RIN.WebAPI.Utils;

public class StoreRequestBodyMiddleware
{
    private readonly RequestDelegate Next;

    public StoreRequestBodyMiddleware(RequestDelegate next)
    {
        Next = next;
    }

    public async Task Invoke(HttpContext context)
    {
        context.Request.EnableBuffering();

        using var reader = new StreamReader(context.Request.Body);
        var body = await reader.ReadToEndAsync();


        // Store the body in HttpContext.Items
        context.Items["RequestBody"] = body;

        // Reset the request body stream position so the next middleware can read it
        context.Request.Body.Position = 0;

        await Next(context);
    }
}
