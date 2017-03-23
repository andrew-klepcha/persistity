﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Persistity.Attributes;
using Persistity.Exceptions;
using Persistity.Extensions;

namespace Persistity.Mappings.Mappers
{
    public class DefaultTypeMapper : TypeMapper
    {
        public override IEnumerable<PropertyInfo> GetPropertiesFor(Type type)
        {
            return base.GetPropertiesFor(type)
                .Where(x => x.HasAttribute<PersistDataAttribute>()&&
                            x.GetSetMethod().IsPublic &&
                            x.GetGetMethod().IsPublic);
        }

        public override TypeMapping GetTypeMappingsFor(Type type)
        {
            if (!type.HasAttribute<PersistAttribute>())
            { throw new TypeNotPersistable(type); }

            return base.GetTypeMappingsFor(type);
        }
    }
}