// OSC Jack - Open Sound Control plugin for Unity
// https://github.com/keijiro/OscJack

using System;
using System.Net;
using System.Net.Sockets;

namespace VMCLight.Osc;
public sealed class OscClient : IDisposable
{
    #region Object life cycle

    public OscClient(string destination, int port)
    {
        _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        if (destination == "255.255.255.255")
            _socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);

        var dest = new IPEndPoint(IPAddress.Parse(destination), port);
        _socket.Connect(dest);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion

    public void Send(string address, string target, float[] element)
    {
        _encoder.Clear();
        _encoder.Append(address);
        _encoder.Append($",s{new string('f',element.Length)}");
        _encoder.Append(target);
        for(int i=0;i<element.Length;i++)
            _encoder.Append(element[i]);
        _socket.Send(_encoder.Buffer, _encoder.Length, SocketFlags.None);
    }

    #region IDispose implementation

    bool _disposed;

    void Dispose(bool disposing)
    {
        if (_disposed) return;
        _disposed = true;

        if (disposing)
        {
            if (_socket != null)
            {
                _socket.Close();
                _socket = null;
            }

            _encoder = null;
        }
    }

    ~OscClient()
    {
        Dispose(false);
    }

    #endregion

    #region Private variables

    OscPacketEncoder _encoder = new OscPacketEncoder();
    Socket _socket;

    #endregion
}