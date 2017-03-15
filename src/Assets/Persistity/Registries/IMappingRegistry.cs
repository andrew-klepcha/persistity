﻿using Persistity.Mappings;

namespace Persistity.Registries
{
    public interface IMappingRegistry
    {
        TypeMapping GetMappingFor<T>() where T : new();
    }
}