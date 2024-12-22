using System.Linq;
using System.Net;
using System.Net.Sockets;
using Microsoft.AspNetCore.Http;

namespace VNPay.Utilities
{
    public static class NetworkHelper
    {
        /// <summary>
        /// Retrieves the client IP address from the HTTP context.
        /// If the client is using IPv6, attempts to resolve the corresponding IPv4 address.
        /// </summary>
        /// <param name="context">The HTTP context of the current request.</param>
        /// <returns>The client's IP address as a string, or an empty string if not resolvable.</returns>
        public static string GetIpAddress(HttpContext context)
        {
            var remoteIpAddress = context?.Connection?.RemoteIpAddress;

            if (remoteIpAddress == null)
            {
                throw new NullReferenceException("IP address not found.");
            }

            return remoteIpAddress.AddressFamily == AddressFamily.InterNetworkV6
                ? GetIPv4FromIPv6(remoteIpAddress)
                : remoteIpAddress.ToString();
        }

        /// <summary>
        /// Resolves an IPv4 address from an IPv6 address, if available.
        /// </summary>
        /// <param name="ipv6Address">The IPv6 address to resolve.</param>
        /// <returns>The resolved IPv4 address as a string, or an empty string if not found.</returns>
        private static string GetIPv4FromIPv6(IPAddress ipv6Address)
        {
            var ipv4Address = Dns.GetHostEntry(ipv6Address)
                .AddressList
                .FirstOrDefault(ip => ip.AddressFamily == AddressFamily.InterNetwork);

            return ipv4Address?.ToString() ?? string.Empty;
        }
    }
}
