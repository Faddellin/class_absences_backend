using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DbModels;

public class Response(string status,
    string message)
{
    public string status { get; set; } = status;

    public string message { get; set; } = message;
}
