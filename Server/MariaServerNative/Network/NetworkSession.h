#pragma once

namespace Maria::Server::Native
{
    class NetworkSession;
    typedef void (*OnSessionAcceptCallbackPtr)(NetworkSession* session);
    typedef void (*OnSessionConnectedCallbackPtr)(NetworkSession* session, const int ec);
    typedef void (*OnSessionDisconnectCallbackPtr)(NetworkSession* session);
    typedef void (*OnSessionReceiveCallbackPtr)(const char* data, size_t length);
    typedef void (*OnSessionSendCallbackPtr)(size_t buffer_count);

    struct NetworkMessageHeader
    {
        int MessageLength;
    };

    class NetworkSession
    {
    public:
        virtual ~NetworkSession() = default;

    public:
        virtual void Start() = 0;
        virtual void Stop() = 0;
        virtual void Send(const char* data, int length) = 0;

    protected:
        virtual void Receive() = 0;
        virtual void OnConnectionWrite(size_t bufferCount) = 0;
        virtual void OnConnectionRead(size_t byteCount) = 0;
        virtual void OnConnectionDisconnect() = 0;

    public:
        void Bind(OnSessionReceiveCallbackPtr onReceive, OnSessionSendCallbackPtr onSend);

    protected:
        OnSessionReceiveCallbackPtr on_receive_callback_ = nullptr;
        OnSessionSendCallbackPtr on_send_callback_ = nullptr;


    };
}

