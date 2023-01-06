using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml.Serialization;
using System.Xml;
using GalaSoft.MvvmLight.Command;

namespace ServidorEncuesta
{
    public class ServidorEncuesta : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        List<TcpClient> clientes = new List<TcpClient>();
        
        List<string> encuestas = new List<string>();
        TcpListener listener;

        Dispatcher dispatcher;

        public ICommand IniciarCommand { get; set; }
        public ICommand DetenerCommand { get; set; }

        private string conexion;
        public string Conexion
        {
            get { return conexion; }
            set
            {
                conexion = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Conexion"));
            }
        }

        public int TotalEncuestas { get { return encuestas.Count; } }

        public float PorcExcelente
        {
            get
            {
                int totalEx = encuestas.Where(x => x == "Excelente").Count();
                if (encuestas.Count > 0)
                    return totalEx * 100 / encuestas.Count;
                else
                    return 0;
            }
        }
        
        public float PorcRegular
        {
            get
            {
                int totalR = encuestas.Where(x => x == "Regular").Count();
                if (encuestas.Count > 0)
                    return totalR * 100 / encuestas.Count;
                else
                    return 0;
            }
        }
        
        public float PorcPesimo
        {
            get
            {
                int totalP = encuestas.Where(x => x == "Pésimo").Count();
                if (encuestas.Count > 0)
                    return totalP * 100 / encuestas.Count;
                else
                    return 0;
            }
        }

        public void IniciarServidor()
        {
            if (listener == null)
            {
                Task.Run(() =>
                {
                    try
                    {
                        IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 5500);
                        listener = new TcpListener(endPoint);
                        listener.Start();

                        while (listener != null)
                        {
                            TcpClient tcp = listener.AcceptTcpClient();
                            clientes.Add(tcp);
                            Recibir(tcp);
                        }
                    }
                    catch (Exception) { }
                });
                conexion = "Servidor conectado";
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
            }
        }
        public void Recibir(TcpClient cliente)
        {
            Task.Run(() =>
            {
                NetworkStream ns = cliente.GetStream();

                while (cliente.Connected)
                {
                    
                    if (cliente.Available > 0)
                    {
                        byte[] buffer = new byte[cliente.Available];
                        ns.Read(buffer, 0, buffer.Length);

                        string encuesta = Encoding.UTF8.GetString(buffer);

                        dispatcher.Invoke(() =>
                        {
                            encuestas.Add(encuesta);

                            Guardar();
                        });
                    }
                    Task.Delay(50);
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                }
            });
        }
        public void DetenerServidor()
        {
            if (listener != null)
            {
                listener.Stop();
                listener = null;

                foreach (var cliente in clientes)
                {
                    cliente.Close();
                }
                clientes.Clear();
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(null));
                conexion = "Servidor desconectado";
            }
        }
        public ServidorEncuesta()
        {
            Cargar();
            dispatcher = Dispatcher.CurrentDispatcher;
            IniciarCommand = new RelayCommand(IniciarServidor);
            DetenerCommand = new RelayCommand(DetenerServidor);
        }

        
        private void Cargar()
        {
            try
            {
                XmlReader archivo = XmlReader.Create("estadisticas.xml");
                XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
                encuestas = (List<string>)serializer.Deserialize(archivo);
                archivo.Close();
            }
            catch (Exception)
            {
                encuestas = new List<string>();
            }
        }
        private void Guardar()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<string>));
            XmlWriter archivo = XmlWriter.Create("estadisticas.xml");

            serializer.Serialize(archivo, encuestas);
            archivo.Close();
        }
    }
}
