﻿using System.Collections.Generic;
using Persistity.Endpoints;
using Persistity.Extensions;
using Persistity.Processors;
using Persistity.Transformers;

namespace Persistity.Pipelines.Builders
{
    public class SendPipelineBuilder
    {
        private ITransformer _transformStep;
        private ISendData _sendDataStep;
        private IList<IProcessor> _processors;

        public SendPipelineBuilder(ITransformer transformStep)
        {
            _transformStep = transformStep;
            _processors = new List<IProcessor>();
        }

        public SendPipelineBuilder WithProcessor(IProcessor processor)
        {
            _processors.Add(processor);
            return this;
        }

        public SendPipelineBuilder WithProcessors(params IProcessor[] processors)
        {
            _processors.AddRange(processors);
            return this;
        }

        public SendPipelineBuilder SendTo(ISendData sendData)
        {
            _sendDataStep = sendData;
            return this;
        }

        public ISendDataPipeline Build()
        {
            return new SendDataPipeline(_transformStep, _sendDataStep, _processors);
        }
    }
}