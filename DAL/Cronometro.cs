using System;using System.Diagnostics;namespace Axon.GFE{    public sealed class Cronometro    {        private static readonly Lazy<Cronometro> lazy = new Lazy<Cronometro>(() => new Cronometro());        public static Cronometro Instancia { get { return lazy.Value; } }        private Cronometro()
        {            _cronometro = new Stopwatch();        }        Stopwatch _cronometro;        public void Iniciar()
        {            if (!_cronometro.IsRunning) _cronometro.Start();        }        public TimeSpan Detener()
        {            if (_cronometro.IsRunning) _cronometro.Stop();            return _cronometro.Elapsed;        }
    }}