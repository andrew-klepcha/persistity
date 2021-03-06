﻿using LazyData.Serialization;
using Persistity.Endpoints;

namespace Persistity.Pipelines.Builders
{
    public class PipelineBuilder
    {
        public SendPipelineBuilder SerializeWith(ISerializer serializer)
        { return new SendPipelineBuilder(serializer); }

        public ReceivePipelineBuilder RecieveFrom(IReceiveDataEndpoint recieveDataEndpoint)
        { return new ReceivePipelineBuilder(recieveDataEndpoint); }
    }
}