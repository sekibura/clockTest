using StarGames.Digger.System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net;
using UnityEngine;
using System.Timers;
using System.Linq;


namespace Sekibura.ClockInterview.System
{
    public class TimeController : IInitializable
    {
        private string[] ntpServers = new string[]
        {
            "pool.ntp.org",
            "time.google.com",
            "time.windows.com",
            "time.nist.gov"
        };

        private DateTime _syncedTime;
        private Timer _secondTimer;
        private Timer _syncTimer;

        public Action<DateTime> TimeUpdatedAction;
        public Action<DateTime> TimeNewMinuteUpdateAction;
        public Action<DateTime> TimeNewHourUpdateAction;

        public void Initialize()
        {
            SyncTime();
            InitSecondTimer();
            InitSyncTimer();
        }

        private void InitSecondTimer()
        {
            _secondTimer = new Timer(1000);            
            _secondTimer.Elapsed += UpdateTime;
            _secondTimer.Start();
        }

        private void InitSyncTimer()
        {
            _syncTimer = new Timer(3600 * 1000); // 1 hour
            _syncTimer.Elapsed += (s,e) => SyncTime();
            _syncTimer.Start();
        }

        private void UpdateTime(object sender, ElapsedEventArgs e)
        {
            _syncedTime = _syncedTime.AddSeconds(1);
            Debug.Log("Текущее время: " + _syncedTime.ToString("HH:mm:ss"));
            TimeUpdatedAction?.Invoke(_syncedTime);

            if(_syncedTime.Second == 0)
                TimeNewMinuteUpdateAction?.Invoke(_syncedTime);

            if(_syncedTime.Minute == 0)
                TimeNewHourUpdateAction?.Invoke(_syncedTime);
        }
        public void SyncTime()
        {
            try
            {
                _syncedTime = GetAverageNetworkTime(ntpServers);
                Debug.Log("Время синхронизировано, срденее время по всем ntp серверам: " + _syncedTime);
            }
            catch (Exception ex)
            {
                Debug.LogError("Ошибка при синхронизации с NTP сервером: " + ex.Message);
                _syncedTime = DateTime.Now;
            }
            TimeUpdatedAction?.Invoke(_syncedTime);
        }

        public DateTime GetDateTime()
        {
            return _syncedTime;
        }

        private DateTime[] GetNetworkTimeFromServers(string[] ntpServers)
        {
            DateTime[] times = new DateTime[ntpServers.Length];

            for (int i = 0; i < ntpServers.Length; i++)
            {
                try
                {
                    times[i] = GetNetworkTime(ntpServers[i]);
                    Console.WriteLine($"Время с сервера {ntpServers[i]}: {times[i]}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при подключении к серверу {ntpServers[i]}: {ex.Message}");
                }
            }

            return times;
        }

        private DateTime GetNetworkTime(string ntpServer)
        {
            var epoch = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            var ntpData = new byte[48];
            ntpData[0] = 0x1B; // Устанавливаем первые байты запроса NTP

            using (var socket = new UdpClient())
            {
                socket.Connect(ntpServer, 123);
                socket.Send(ntpData, ntpData.Length);
                var a = new IPEndPoint(IPAddress.Any, 0);
                var response = socket.Receive(ref a);

                ulong intPart = (ulong)response[40] << 24 | (ulong)response[41] << 16 | (ulong)response[42] << 8 | (ulong)response[43];
                ulong fractPart = (ulong)response[44] << 24 | (ulong)response[45] << 16 | (ulong)response[46] << 8 | (ulong)response[47];

                ulong milliseconds = (intPart * 1000) + ((fractPart * 1000) / 0x100000000L);
                var networkDateTime = epoch.AddMilliseconds((long)milliseconds);

                return networkDateTime.ToLocalTime();
            }
        }

        private DateTime GetAverageNetworkTime(string[] ntpServers)
        {
            DateTime[] times = new DateTime[ntpServers.Length];

            for (int i = 0; i < ntpServers.Length; i++)
            {
                try
                {
                    times[i] = GetNetworkTime(ntpServers[i]);
                    Console.WriteLine($"Время с сервера {ntpServers[i]}: {times[i]}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при подключении к серверу {ntpServers[i]}: {ex.Message}");
                }
            }

            // Вычисляем среднее значение
            long averageTicks = (long)times.Average(time => time.Ticks);
            return new DateTime(averageTicks);
        }

       public void Dispose()
       {
            _secondTimer.Stop();
            _syncTimer.Stop();
            _secondTimer.Dispose();
            _syncTimer.Dispose();
       }
    }
}
