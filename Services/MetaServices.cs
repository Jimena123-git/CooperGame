using CooperGame.Data;
using CooperGame.Models;



namespace CooperGame.Services
{
    public class MetaServices
    {
        private readonly Random random = new Random();
        private readonly AppDbContext _context;

        public  MetaServices (AppDbContext context)
        {
            _context = context;
         
        }
        //25/09
        public void GenerarMetasV1(Partida partida, double factorDificultad)
        {
            partida.Recursos ??= new List<Recurso>();

            foreach (TipoRecurso tipo in Enum.GetValues(typeof(TipoRecurso)))
            {
                int meta;
                do
                {
                    meta = (int)Math.Round(random.NextDouble() * factorDificultad * 100);
                }
                while (meta <= 10 || meta >= 100);

                Recurso recurso = new Recurso
                {
                    Tipo = tipo,
                    Meta = meta,
                    CantidadRecolectada = 0,
                    Partida = partida 
                };

                partida.Recursos.Add(recurso);
            }

      
        }


        public void GenerarMetasV2(Partida partida, double factorDificultad)
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
}

