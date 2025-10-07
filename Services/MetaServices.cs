    using CooperGame.Data;
using CooperGame.Models;
using System;

namespace CooperGame.Services
{
    public class MetaServices
    {
        private static readonly Random random = new Random();
        private readonly AppDbContext _context;

        public MetaServices(AppDbContext context)
        {
            _context = context;
        }

        public int GenerarMetasPorPartida()
        {
            int MetaXRecurso = GenerarMeta();

            return MetaXRecurso;

        }


        public int GenerarMeta()
        {
            double numero = random.NextDouble(); 
            int meta = (int)Math.Floor(numero * 100);

            if (meta < 10) meta = 10;
            if (meta > 100) meta = 100;

            return meta;
        }
    }
}


       /* public void GenerarMetasV2(Partida partida, double factorDificultad)
        {
            partida.Recursos ??= new List<Recurso>();

            foreach (TipoRecurso tipo in Enum.GetValues(typeof(TipoRecurso)))
            {
                int meta;

                do
                {
                    meta = (int)Math.Round(random.NextDouble() * factorDificultad * 10);
                }
                while (meta <= 10 || meta >= 1);

                Recurso recurso = new Recurso
                {
                    Tipo = tipo,
                    Meta = meta,
                    CantidadRecolectada = 0,
                    IdPartida = partida.IdPartida,
                    Partida = partida
                };
                partida.Recursos.Add(recurso);

                _context.Recursos.Add(recurso);
            }

            _context.SaveChanges();
        }
    }
}*/