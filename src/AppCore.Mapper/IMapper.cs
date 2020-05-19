using System;
using System.Collections.Generic;
using System.Text;

namespace AppCore.Mapper
{
    public interface IMapper
    {
        TDestination Map<TSource, TDestination>(TSource source) where TDestination : class where TSource : class;
    }
}
