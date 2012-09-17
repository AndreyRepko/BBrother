using System;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using MonitorServerApplication.Packets;

namespace MonitorServerApplication.ServerThreading
{
    // Класс-обработчик клиента
    internal class ClientThread
    {
        private readonly NetworkStream _clientStream;
        private readonly TcpClient _client;

        /// <summary>
        ///  Set it to true for thread stop
        /// </summary>
        public bool IsShouldFinishWork;

        public bool IsFinished;

        private void GetInfoMessage()
        {
            var dataSize = new byte[4];
            _clientStream.Read(dataSize, 0, 4);
            int iDataSize = BitConverter.ToInt32(dataSize, 0);
            var packetData = new byte[iDataSize];
            _clientStream.Read(packetData, 0, iDataSize);

            DataDecoder.InfoMessageDecoder(packetData);

            byte[] b2 = System.Text.Encoding.ASCII.GetBytes(OldProtocolConst.Con_OK);
            _clientStream.Write(b2, 0, 2);
        }

        private void SendSettings()
        {
            var dataSize = new byte[4];
            _clientStream.Read(dataSize, 0, 4);
            int iDataSize = BitConverter.ToInt32(dataSize, 0);
            
            if (iDataSize!=4)
                throw new SystemException("Panicum! SendSettings failed couse if size mismatch");

            var settingsType = new byte[iDataSize];
            _clientStream.Read(settingsType, 0, iDataSize);

            int iSettingsType = BitConverter.ToInt32(dataSize, 0);
            

            var settingsSize = new byte[4];
            //settingsSize[0] = 1;
            _clientStream.Write(settingsSize, 0, 4);
        
            Thread.Sleep(5);

            //TODO: Write sttings sendong block
            //clData->SendSettings(count);

            var confirmation = new byte[2];
            _clientStream.Read(confirmation, 0, 2);

            if ((confirmation[0] == OldProtocolConst.Con_OK[0]) && 
                (confirmation[1] == OldProtocolConst.Con_OK[1]))
               {
                 //TODO: log all is OK
               } else
               {
                //TODO: log settings failed
               }
        }

        private void DoConnectionEnd()
        {
            byte[] b2 = System.Text.Encoding.ASCII.GetBytes(OldProtocolConst.Con_OK);
            _clientStream.Write(b2, 0, 2);
        }

        private void GetPacketData(bool wasItTimer)
        {
            /*if ( Sign == Con_Data_WindChng )  {
                sComm = AnsiString("Переключение по смене окна ");
                clData->TypPack = 10;
                }
             else
            if ( Sign == Con_Data_Timer)  {
                sComm = AnsiString("Переключение по таймеру.");
                clData->TypPack = 12;
                };*/

            var dataSize = new byte[4];
            _clientStream.Read(dataSize, 0, 4);
            int iDataSize = BitConverter.ToInt32(dataSize, 0);
            var packetData = new byte[iDataSize];
            _clientStream.Read(packetData, 0, iDataSize);

            DataDecoder.PacketDecoder(packetData);
            
            byte[] b2 = System.Text.Encoding.ASCII.GetBytes(OldProtocolConst.Con_OK);
            _clientStream.Write(b2, 0, 2);
        }

        // Конструктор класса. Ему нужно передавать принятого клиента от TcpListener
        public ClientThread(TcpClient client)
        {
            _client = client;
            _clientStream = client.GetStream();

            _clientStream.ReadTimeout = 1000;

            var signature = new byte[4];
            _clientStream.Read(signature, 0, 4);

            switch (BitConverter.ToInt32(signature, 0))
            {
                case OldProtocolConst.Con_Begin:
                    var readBuffer = new byte[220];
                    _clientStream.Read(readBuffer, 0, 220);
                    var pcName = System.Text.Encoding.GetEncoding(1251).GetString(readBuffer, 0, 100);
                    var ip = System.Text.Encoding.GetEncoding(1251).GetString(readBuffer, 100, 20);
                    var userName = System.Text.Encoding.GetEncoding(1251).GetString(readBuffer, 120, 100);
                    //string s = new string(read_buffer, 0, 100);
                    break;
                case OldProtocolConst.Con_End:
                    break;
                default:
                    throw new SystemException("Unknown signature");
            }
        }

        public void Execute()
        {
            while (!IsShouldFinishWork)
            {
                try
                {
                    if (!_clientStream.DataAvailable)
                    {
                        Thread.Sleep(10);
                        continue;
                    }
                    var signature = new byte[4];
                    _clientStream.Read(signature, 0, 4);
                    int iSignature = BitConverter.ToInt32(signature, 0);
                    switch (iSignature)
                    {
                        // завершение работы
                        case OldProtocolConst.Con_End:
                            IsFinished = true;
                            IsShouldFinishWork = true;
                            DoConnectionEnd();
                            break;
                        // оперативные данные.    
                        case OldProtocolConst.Con_Data_Timer:
                            GetPacketData(true);
                            break;
                        case OldProtocolConst.Con_Data_WindChng:
                            GetPacketData(false);
                            break;
                        //информационное актуальное сообщение
                        case OldProtocolConst.Con_Info_Msg:
                            GetInfoMessage();
                            break;
                        //кусок архива
                        case OldProtocolConst.Con_Data_Archive:
                            throw new SystemException("Not implement yet");

                            #region ARCHIVE commented

                            /* sComm = AnsiString("Архивное сообщение.");
                            // Вычитываем количество байт на будущее
                            if( ReadData(clSock, (char*)&tc, 4) == 0 )
                             {
                               if (tc!=0)
                                  {
                                    if (mem!=NULL)
                                        free(mem);
                                    mem=(char *)malloc(tc);
                                  }

                                else
                                  clData->SynForScreen("Пустой 0-й пакет! TO6");

                               //Теперь читаем данные.
                               ReadData(clSock, mem, tc);

                               if (tc<4)
                                {
                                    clData->SynForScreen("Ошибка в пакете! Свяжитесь с разработчиками! TO7");
                                    IsSend = (WriteData(clSock, Con_Failed, 2 ) == 0 );
                                    free(mem);
                                    mem=NULL;
                                    IsSend=false;
                                    break;
                                };

                              count=*(int *)mem;
                              bt=4;
                              for (int i=0;i<count;i++)
                               {
                                   //проверяем длину
                                   if ((bt+2*sizeof(int))>tc)
                                    {
                                      clData->SynForScreen("Ошибка в пакете! Свяжитесь с разработчиками! TO8");
                                      IsSend = (WriteData(clSock, Con_Failed, 2 ) == 0 );
                                      free(mem);
                                      mem=NULL;                              
                                      IsSend=false;
                                      break;
                                    };
                                   tp=*(int *)(mem+bt);
                                   bt+=sizeof(int);
                                   rs=*(int *)(mem+bt);
                                   bt+=sizeof(int);

                                   //проверяем длину
                                   if (bt+rs>tc)
                                    {
                                      clData->SynForScreen("Ошибка в пакете! Свяжитесь с разработчиками! TO9");
                                      IsSend = (WriteData(clSock, Con_Failed, 2 ) == 0 );                              
                                      free(mem);
                                      mem=NULL;                              
                                      IsSend=false;
                                      break;
                                    };

                                   if (Pack_Inform==tp)
                                    {
                                     if (ParseInfoPacket((mem+bt),clData->msg,rs) == 0)
                                      {
                                        clData->SynForScreen("Информационное сообщение. Архив.");
                                        clData->msg->rt=2;
                                        clData->IsErr=0;
                                        clData->InfoSaveData();
                                        delete clData->msg;
                                        clData->msg=NULL;
                                      }else
                                      {
                                        clData->SynForScreen("Ошибка в пакете! Свяжитесь с разработчиками! TO10");
                                        IsSend = (WriteData(clSock, Con_Failed, 2 ) == 0 );                                
                                        //free(mem);
                                        IsSend=false;
                                        break;
                                      };
                                      if (0!=clData->IsErr)
                                       {
                                        clData->SynForScreen("Ошибка в пакете! Свяжитесь с разработчиками! TO11");
                                        //IsSend = (WriteData(clSock, Con_Failed, 2 ) == 0 );
                                        //free(mem);
                                        IsSend=true;
                                        //break;
                                       };
                                     bt+=rs;
                                    } else
                                   if (Pack_Data==tp)
                                   {
                                      if (ParsePacket2(mem+bt,clData->Pack,tc) == 0)
                                      {
                                         clData->TypPack=TR_Con_Data_Archive;

                                         clData->SynForScreen(AnsiString("Сообщение с данными. Архив. "));
                                         clData->IsErr=0;
                                         clData->MainSaveData();
                                        //чистим память
                                         delete clData->Pack;
                                         clData->Pack=NULL;
                                         if (0!=clData->IsErr)
                                          {
                                            clData->SynForScreen("Ошибка в пакете! Свяжитесь с разработчиками! TO12");
                                            //IsSend = (WriteData(clSock, Con_Failed, 2 ) == 0 );
                                            //free(mem);
                                            IsSend=true;
                                            break;
                                          }
                                      } else
                                      {
                                        clData->SynForScreen("Ошибка в пакете! Свяжитесь с разработчиками! TO13");
                                        IsSend = (WriteData(clSock, Con_Failed, 2 ) == 0 );
                                        //free(mem);
                                        IsSend=false;
                                        break;
                                      }
                                     bt+=rs; 
                                   } else
                                   {
                                    bt+=rs;
                                   };
                                }
                             free(mem);
                             mem=NULL;                     
                             } else
                                    {
                                      clData->SynForScreen("Ошибка в пакете! Свяжитесь с разработчиками! TO14");
                                      IsSend=false;
                                      break;
                                    };
                            
                            IsSend = (WriteData(clSock, Con_OK, 2 ) == 0 );
                            if ( IsShouldFinishWork )  loop_ = false;
                            //конец архивной записи*/
                            //break;

                            #endregion

                        case OldProtocolConst.Con_File:
                            throw new SystemException("Not implement yet");

                            #region FILES commented

                            /*  clData->SendCurrentFiles(0);
                                if( ReadData(clSock, (char*)&tc, 4) == 0 )
                                 {
                                   if (tc>0)
                                      {
                                        if (mem!=NULL)
                                          free(mem);
                                        mem=(char *)malloc(tc);
                                      }

                                    else
                                     {
                                       clData->SynForScreen("Пустой 0-й пакет!");
                                     }
                                   //Теперь читаем данные.
                                   ReadData(clSock, mem, tc);
                                   cnt=*(int *)mem;
                                   bt=4;

                                   if ((cnt<0)|| (cnt>MAX_NUMB_FILES))
                                      {
                                          clData->SynForScreen("Ошибка в пакете! Свяжитесь с разработчиками! TO15");
                                          IsSend = (WriteData(clSock, Con_Failed, 2 ) == 0 );
                                          //free(mem);
                                          IsSend=false;
                                          loop_ = false;
                                          break;
                                      }

                                   clData->lst->Clear();
                                   for (int i=0;i<cnt;i++)
                                    {
                                     size=*(int *)(mem+bt);
                                     bt+=sizeof(int);
                                     if ((bt>tc) || (bt+size>tc) || (tc<0))
                                      {
                                          clData->SynForScreen("Ошибка в пакете! Свяжитесь с разработчиками! TO16");
                                          IsSend = (WriteData(clSock, Con_Failed, 2 ) == 0 );
                                          //free(mem);
                                          IsSend=false;
                                          loop_ = false;
                                          break;
                                      }
                                     clData->lst->Add(AnsiString((mem+bt),size));
                                     bt+=size;
                                    }
                                   free(mem);
                                   mem=NULL;
                                   clData->SendCurrentFiles(1);
                                   ReadData(clSock, IsOk, 2);
                                   IsOk[2]=0x0;
                                   if ((Con_OK[0]==IsOk[0]) && (Con_OK[1]==IsOk[1]))
                                      {
                                       //TODO write to DB
                                       clData->SynForScreen("Файлы переданы успешно.");
                                      } else
                                      {
                                       clData->SynForScreen("Файлы не переданы");
                                      }
                                 } else
                                 {
                                    clData->SynForScreen("Обновление не удалось");
                                 }*/
                            // break;

                            #endregion

                        case OldProtocolConst.Con_Sett_Reqest:
                            SendSettings();
                            break;
                        default:
                            //TODO: LOg something here
                            throw new SystemException("Not implement yet");
                    }
                }
                catch (IOException)
                {
                    //TODO: log exception to the log )
                }
            }

            // Закроем соединение
            _client.Close();
            IsFinished = true;
        }

    }
}
