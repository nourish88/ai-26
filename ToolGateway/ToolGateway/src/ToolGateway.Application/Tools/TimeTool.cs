using ModelContextProtocol.Server;
using System.ComponentModel;

namespace ToolGateway.Application.Tools
{
    [McpServerToolType]
    public sealed class TimeTool
    {
        [McpServerTool, Description("Return the current local date (yyyy-MM-dd).")]
        public static string GetCurrentDate() =>
            DateTime.Now.ToString("yyyy-MM-dd");

        [McpServerTool, Description("Return the current local time (HH:mm:ss).")]
        public static string GetCurrentTime() =>
            DateTime.Now.ToString("HH:mm:ss");

        [McpServerTool, Description("Return the current UTC time (HH:mm:ss).")]
        public static string GetCurrentUtcTime() =>
            DateTime.UtcNow.ToString("HH:mm:ss");
    }
}
