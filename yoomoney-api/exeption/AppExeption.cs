namespace yoomoney_api.exeption;

public class AppException : Exception
{
    public int? StatusCode { get; set; }
    public override string? Message { get; }
    public string? Details { get; set; }
        
    public AppException(int? statusCode = null,string? message = null,string? details = null)
    {
        switch (message)
        {
            case var s when s == "InvalidToken":
                Message = "\n\nInstruct => Token is not valid, or does not have the appropriate rights\n";
                break;
            case var s when s == "IllegalParamType":
                Message = "\n\nInstruct => Invalid parameter value 'type'\n";
                break;
            case var s when s == "IllegalParamStartRecord":
                Message = "\n\nInstruct => Invalid parameter value 'start_record'\n";
                break;
            case var s when s == "IllegalParamRecords":
                Message = "\n\nInstruct => Invalid parameter value 'records'\n";
                break;
            case var s when s == "IllegalParamLabel":
                Message = "\n\nInstruct => Invalid parameter value 'label'\n";
                break;
            case var s when s == "IllegalParamFromDate":
                Message = "\n\nInstruct => Invalid parameter value 'from_date'\n";
                break;
            case var s when s == "IllegalParamTillDate":
                Message = "\n\nInstruct => Invalid parameter value 'till_date'\n";
                break;
            case var s when s == "IllegalParamOperationId":
                Message = "\n\nInstruct => Invalid parameter value 'operation_id'\n";
                break;
            case var s when s == "TechnicalError":
                Message = "\n\nInstruct => Technical error, try calling the operation again later\n";
                break;
            case var s when s == "UnauthorizedClient":
                Message = "\n\nInstruct => Required query parameters are missing or have incorrect or invalid values\n";
                break;
            case var s when s == "InvalidRequest":
                Message = "\n\nInstruct => Invalid parameter value 'client_id' or 'client_secret', or the application" + 
                          " does not have the right to request authorization (for example, YooMoney blocked it 'client_id')\n";
                break;
            case var s when s == "InvalidGrant":
                Message = "\n\nInstruct => In issue 'access_token' denied. YuMoney did not issue a temporary token, " +
                          "the token is expired, or this temporary token has already been issued " +
                          "'access_token' (repeated request for an authorization token with the same temporary token)\n";
                break;
            case var s when s == "EmptyToken":
                Message = "\n\nInstruct => Response token is empty. Repeated request for an authorization token\n";
                break;
            case var s when s == "EmptyRedirectUrlCode":
                Message = "\n\nInstruct => Response url-code is empty or ended limit live time. Repeated request for an url-code\n";
                break;
        }
        StatusCode = statusCode;
        Details = details;
    } 
}