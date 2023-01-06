using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Encuesta
{
    [Serializable]
    public class ClienteEncuesta
    {
        TcpClient cliente = new TcpClient();

        public ICommand ConectarCommand { get; set; }
        public ICommand EnviarCommand { get; set; }

        private bool conectado = false;

        public bool Conectado
        {
            get { return conectado; }
            set { conectado = value; }
        }
        private void Conectar()
        {
            if (Conectado == false)
            {
                cliente.Connect(new IPEndPoint(IPAddress.Loopback, 5500));
                Task.Delay(15);
                Conectado = cliente.Connected;
            }
        }
        private void Enviar(string encuesta)
        {
            if (conectado == true)
            {
                if (!string.IsNullOrWhiteSpace(encuesta))
                {
                    NetworkStream ns = cliente.GetStream();
                    var buffer = Encoding.UTF8.GetBytes(encuesta);
                    ns.Write(buffer, 0, buffer.Length);
                }
            }
            else
                MessageBox.Show("Cliente no conectado.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public ClienteEncuesta()
        {
            ConectarCommand = new RelayCommand(Conectar);
            EnviarCommand = new RelayCommand<string>(Enviar);
        }

    }
}

