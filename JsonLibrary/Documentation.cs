namespace JsonLibrary
{
    public class Documentation
    {
        private const string kFileName = "Documentation.json";

        public String Name { get; set; }
        public List<Maquina> Maquinas { get; set; }

        public Documentation() {}

        public bool LoadFromJson()
        {
            // cargar la informacion del json
            return false;
        }
        
        /// <summary>
        /// Lanza <see cref="NullReferenceException"/> si las maquinas no han sido cargadas, o <see cref="ArgumentOutOfRangeException"/> si el indice pasado no corresponde con la lista.
        /// </summary>
        /// <param name="index"></param>
        /// <returns>La maquina que corresponda a la posicion del indice pasado parametro</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public Maquina GetMaquina(int index)
        {
            if (Maquinas == null) throw new NullReferenceException();
            if (index >= Maquinas.Count || index < 0) throw new ArgumentOutOfRangeException();
            return Maquinas[index];
        }

        /// <summary>
        /// Lanza <see cref="NullReferenceException"/> si las maquinas no han sido cargadas, o <see cref="ArgumentNullException"/> si la maquina pasada como ejemplo es null, o <see cref="NotExistsException"/> si la maquina no existe en la lista
        /// </summary>
        /// <param name="example"></param>
        /// <returns>La maquina que coincida con el ejemplo pasado por parametro</returns>
        /// <exception cref="NullReferenceException"></exception>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <exception cref="NotExistsException"></exception>
        public Maquina GetMaquina(Maquina example)
        {
            if (Maquinas == null || example == null) throw new NullReferenceException();
            if (example == null) throw new ArgumentNullException();
            if (Maquinas.Count == 0) throw new ArgumentOutOfRangeException();
            foreach (Maquina maquina in Maquinas) if (maquina.Name == example.Name) return maquina;
            throw new NotExistsException();
        }
    }

    public class Maquina
    {
        public String Name { set; get; }
        public List<Documento> Partes { get; set; }
    }

    public class Documento
    {
        public String Name { set; get; }
        public String Path { set; get; }
    }
}