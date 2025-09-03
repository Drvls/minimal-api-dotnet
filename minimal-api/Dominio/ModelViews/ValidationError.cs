//Listen to "Yun li - Validation"
using System.Collections.Generic;

namespace minimal_api.Dominio.ModelViews;

public struct ValidationError
{
    public List<string> Mensagens { get; set; }
}