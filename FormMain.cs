using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;

namespace Pol_IPw
{
	/// <summary>
	/// BufferIN Class if for reciving the data from the Socket
	/// and parsing it.
	/// </summary>
	public class BufferIN
	{
		public event System.EventHandler OnNewData; 
		public event System.EventHandler OnError;
		public byte[] Data;
		public string Error;
		public int LinkSpeed;
		public int ActualSpeed;
		public int AsingSpeed;
		public bool Report;
		
		public BufferIN()
		{
			this.Data=new byte[8192];
		}
		public void Parser()
		{
			string IN;
			IN = System.Text.Encoding.Default.GetString(this.Data);
				
			if(Char.IsNumber(IN,1))//If the packet recive has number start parser
			{
				int Index;
				string StringHost;

				Index=IN.IndexOf(" ",0); //On The first space is linkspeed
				if(Index>0 && Index!=1)
				{
					LinkSpeed=Convert.ToInt32(IN.Substring(0,Index));
				}
				else
				{
					Error="Error 104";
					if (OnError != null)
						OnError(this,new System.EventArgs());
				}
				
				StringHost=System.Net.Dns.GetHostName();//Find your ip
				System.Net.IPHostEntry ipEntry = System.Net.Dns.GetHostByName (StringHost);
				System.Net.IPAddress [] addr = ipEntry.AddressList;

				for (int i = 0; i < addr.Length; i++)//For looking in all yours ip first match go
				{
					Index=IN.IndexOf(addr[i].ToString(),0);
					if(Index>0 && Index!=1)
					{
						break;
					}
				}
				//Start parser for actualspeed and asingspeed
				if(Index>0 && Index!=1)
				{
					int Index1;
					int Index2;
					Index1=IN.IndexOf("/",Index);
					if(Index1>0 && Index1!=Index)
					{
						Index1++;
						Index2=IN.IndexOf("/",Index1);
						if(Index2>1 && Index2!=Index)
						{
							ActualSpeed=Convert.ToInt32(IN.Substring(Index1,Index2-Index1));
						}
						else
						{
							Error="Error 107";
							if (OnError != null)
								OnError(this,new System.EventArgs());
						}

						Index1=Index2;
						Index1++;
						Index2=IN.IndexOf("/",Index1);
						if(Index2>0 && Index2!=Index1)
						{
							string temp=IN.Substring(Index1,Index2-Index1);
							AsingSpeed=Convert.ToInt32(IN.Substring(Index1,Index2-Index1));
							
							if (OnNewData != null)//Rise the event for update the form
								OnNewData(this,new System.EventArgs());
						}
						else
						{
							Error="Error 108";
							if (OnError != null)
								OnError(this,new System.EventArgs());
						}

						
					}
					else
					{
						Error="Error 106";
						if (OnError != null)
							OnError(this,new System.EventArgs());
					}

				}
				else
				{
					Error="Error 105";
					if (OnError != null)
						OnError(this,new System.EventArgs());
				}
			}
			else if(IN.StartsWith("Report"))//If recive a report message rise onnewData
			{
				this.Report=true;
				if (OnNewData != null)
					OnNewData(this,new System.EventArgs());
			}
			else//Socket unblock with no data
			{
				Error="Error 101";
				if (OnError != null)
					OnError(this,new System.EventArgs());
			}


		}

	}
		/// <summary>
		/// Summary description for FormMain.
		/// </summary>
		public class FormMain : System.Windows.Forms.Form
		{
			private System.Windows.Forms.ProgressBar progressBarSpeed;
			private System.Windows.Forms.PictureBox pictureBoxMark;
			private System.ComponentModel.IContainer components;
			private System.Net.Sockets.Socket UdpIn;
			private System.Net.Sockets.UdpClient UdpOut;
			private System.Windows.Forms.Timer timerSocket;
			private BufferIN BufferINSocket;
			private System.Windows.Forms.StatusBarPanel statusBarPanelDown;
			private System.Windows.Forms.StatusBarPanel statusBarPanelStatus;
			private System.Windows.Forms.StatusBar statusBarStatus;
			private System.Windows.Forms.ToolTip toolTipDobleClick;
			private uint count=0;//Counter times of socket realese with no data in

			
						
			public FormMain()
			{
				//
				// Required for Windows Form Designer support
				//
				InitializeComponent();
				this.UdpIn = new System.Net.Sockets.Socket(System.Net.Sockets.AddressFamily.InterNetwork,System.Net.Sockets.SocketType.Dgram,System.Net.Sockets.ProtocolType.Udp);
				this.UdpOut = new System.Net.Sockets.UdpClient();
				BufferINSocket = new BufferIN();
				
				this.UdpIn.Blocking = false;//Socket don't block on recive
				this.UdpIn.Bind(new System.Net.IPEndPoint(System.Net.IPAddress.Any,20002));
				this.UdpIn.SetSocketOption(System.Net.Sockets.SocketOptionLevel.IP,System.Net.Sockets.SocketOptionName.AddMembership,new System.Net.Sockets.MulticastOption(System.Net.IPAddress.Parse("230.0.0.2")));
				
				this.BufferINSocket.OnNewData += new System.EventHandler(this.OnNewData_Event);
				this.BufferINSocket.OnError += new System.EventHandler(this.OnError_Event);
				
				this.timerSocket.Tick += new System.EventHandler(this.timerSocket_Tick);				
				this.SetStyle(System.Windows.Forms.ControlStyles.StandardClick,true);
				
				
				Screen Pantalla = System.Windows.Forms.Screen.PrimaryScreen;
				System.Drawing.Rectangle ScreenBound=Pantalla.Bounds;
				this.SetDesktopLocation(ScreenBound.Width-this.Width-25,25);
				this.toolTipDobleClick.SetToolTip(this,"DoubleClick->Minimize");
				this.toolTipDobleClick.SetToolTip(this.progressBarSpeed,"DoubleClick->Minimize");

				//
				// TODO: Add any constructor code after InitializeComponent call
				//
			}

			/// <summary>
			/// Clean up any resources being used.
			/// </summary>
			protected override void Dispose( bool disposing )
			{
				if( disposing )
				{
					if (components != null) 
					{
						components.Dispose();
					}
				}
				base.Dispose( disposing );
			}

		#region Windows Form Designer generated code
			/// <summary>
			/// Required method for Designer support - do not modify
			/// the contents of this method with the code editor.
			/// </summary>
			private void InitializeComponent()
			{
				this.components = new System.ComponentModel.Container();
				System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(FormMain));
				this.progressBarSpeed = new System.Windows.Forms.ProgressBar();
				this.pictureBoxMark = new System.Windows.Forms.PictureBox();
				this.timerSocket = new System.Windows.Forms.Timer(this.components);
				this.statusBarStatus = new System.Windows.Forms.StatusBar();
				this.statusBarPanelDown = new System.Windows.Forms.StatusBarPanel();
				this.statusBarPanelStatus = new System.Windows.Forms.StatusBarPanel();
				this.toolTipDobleClick = new System.Windows.Forms.ToolTip(this.components);
				((System.ComponentModel.ISupportInitialize)(this.statusBarPanelDown)).BeginInit();
				((System.ComponentModel.ISupportInitialize)(this.statusBarPanelStatus)).BeginInit();
				this.SuspendLayout();
				// 
				// progressBarSpeed
				// 
				this.progressBarSpeed.Location = new System.Drawing.Point(6, 10);
				this.progressBarSpeed.Name = "progressBarSpeed";
				this.progressBarSpeed.Size = new System.Drawing.Size(112, 8);
				this.progressBarSpeed.TabIndex = 2;
				// 
				// pictureBoxMark
				// 
				this.pictureBoxMark.BackColor = System.Drawing.Color.Red;
				this.pictureBoxMark.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
				this.pictureBoxMark.Location = new System.Drawing.Point(6, 4);
				this.pictureBoxMark.Name = "pictureBoxMark";
				this.pictureBoxMark.Size = new System.Drawing.Size(8, 21);
				this.pictureBoxMark.TabIndex = 3;
				this.pictureBoxMark.TabStop = false;
				// 
				// timerSocket
				// 
				this.timerSocket.Enabled = true;
				this.timerSocket.Interval = 1000;
				this.timerSocket.Tick += new System.EventHandler(this.timerSocket_Tick);
				// 
				// statusBarStatus
				// 
				this.statusBarStatus.Dock = System.Windows.Forms.DockStyle.None;
				this.statusBarStatus.Location = new System.Drawing.Point(0, 26);
				this.statusBarStatus.Name = "statusBarStatus";
				this.statusBarStatus.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
																								   this.statusBarPanelDown,
																								   this.statusBarPanelStatus});
				this.statusBarStatus.ShowPanels = true;
				this.statusBarStatus.Size = new System.Drawing.Size(122, 18);
				this.statusBarStatus.SizingGrip = false;
				this.statusBarStatus.TabIndex = 4;
				this.statusBarStatus.DoubleClick += new System.EventHandler(this.FormMain_DoubleClick);
				// 
				// statusBarPanelDown
				// 
				this.statusBarPanelDown.Text = "0.00 Kb/s";
				this.statusBarPanelDown.Width = 55;
				// 
				// statusBarPanelStatus
				// 
				this.statusBarPanelStatus.Width = 65;
				// 
				// toolTipDobleClick
				// 
				this.toolTipDobleClick.AutoPopDelay = 3000;
				this.toolTipDobleClick.InitialDelay = 500;
				this.toolTipDobleClick.ReshowDelay = 500;
				this.toolTipDobleClick.ShowAlways = true;
				// 
				// FormMain
				// 
				this.AutoScale = false;
				this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
				this.ClientSize = new System.Drawing.Size(120, 36);
				this.Controls.AddRange(new System.Windows.Forms.Control[] {
																			  this.statusBarStatus,
																			  this.progressBarSpeed,
																			  this.pictureBoxMark});
				this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
//				this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
				this.MaximizeBox = false;
				this.MinimizeBox = false;
				this.Name = "FormMain";
				this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
				this.Text = "Pol-IP";
				this.TopMost = true;
				this.Closing += new System.ComponentModel.CancelEventHandler(this.FormMain_Closing);
				this.Load += new System.EventHandler(this.FormMain_Load);
				this.DoubleClick += new System.EventHandler(this.FormMain_DoubleClick);
				((System.ComponentModel.ISupportInitialize)(this.statusBarPanelDown)).EndInit();
				((System.ComponentModel.ISupportInitialize)(this.statusBarPanelStatus)).EndInit();
				this.ResumeLayout(false);

			}
		#endregion
			/// <summary>
			/// The main entry point for the application.
			/// </summary>
			[STAThread]
			static void Main() 
			{
				FormMain PolIPw = new FormMain();
				Application.Run(PolIPw);
			}

			
			private void timerSocket_Tick(object sender, System.EventArgs e)
			{
				try
				{
					this.UdpIn.Receive(this.BufferINSocket.Data,this.BufferINSocket.Data.Length,0);
				}
				catch(Exception ex)
				{
					this.BufferINSocket.Data=System.Text.Encoding.Default.GetBytes(ex.Message);
				}
				finally
				{
					this.BufferINSocket.Parser();
				}
			}
			private void OnNewData_Event(object sender, System.EventArgs e)
			{
				this.count=0;//Reset the counter of no data in

				if(this.BufferINSocket.Report)//If report is true send start to the server
				{
					this.Enviar("Start");
					this.BufferINSocket.Report=false;
				}
				else
				{
					float FSpeed;
					this.progressBarSpeed.Maximum=this.BufferINSocket.LinkSpeed;
					if(this.BufferINSocket.ActualSpeed > this.progressBarSpeed.Maximum)
						this.progressBarSpeed.Value=this.progressBarSpeed.Maximum;
                    else
						this.progressBarSpeed.Value=this.BufferINSocket.ActualSpeed;
					//Calculate the place of the mark in the progressbar
					this.pictureBoxMark.Left=this.progressBarSpeed.Left
						+ ((this.BufferINSocket.AsingSpeed * this.progressBarSpeed.Width)
						/this.BufferINSocket.LinkSpeed);
					
					FSpeed=System.Convert.ToSingle(this.BufferINSocket.ActualSpeed);
					FSpeed=FSpeed/1024;
					this.statusBarPanelDown.Text=FSpeed.ToString("F") + " Kb/s";
					this.statusBarPanelStatus.Text="";
					if(this.WindowState==System.Windows.Forms.FormWindowState.Normal)//If the windows is minimize send text to title
					{
						this.Text="Pol-IP";
					}
					else
					{
						this.Text=this.statusBarPanelDown.Text;
					}
				}
			}
			private void OnError_Event(object sender, System.EventArgs e)
			{
				//If error is socket unblock with no data
				if(this.BufferINSocket.Error=="Error 101")
				{
					//If it has been 10 time probably server is off
					if(count>10)
					{
						this.statusBarPanelStatus.Text="No Server";
						this.statusBarPanelDown.Text="";
						this.pictureBoxMark.Left=this.progressBarSpeed.Left;
						this.progressBarSpeed.Value=0;
						this.Enviar("Start");
					}
					else
					{
						if(count==0)//If 0 means data arrived so clean errors
							this.statusBarPanelStatus.Text="";
						else if(count==3)//One time say no data warning
							this.statusBarPanelStatus.Text="No Data";
						else if(count>4)//More times draw points
							this.statusBarPanelStatus.Text=this.statusBarPanelStatus.Text + ".";
						count++;
					}
				}
				else
				{
					//Another error show it
					this.progressBarSpeed.Value=0;
					this.pictureBoxMark.Left=this.progressBarSpeed.Left;
					this.statusBarPanelStatus.Text=this.BufferINSocket.Error;
					this.statusBarPanelDown.Text="";
					this.count=5;
				}
			}

			private void FormMain_Load(object sender, System.EventArgs e)
			{
				//Send start message to server
				this.Enviar("Start");
			}

			private void FormMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
			{
				//Send stop message on exit
				this.Enviar("Stop");
				
			}

			private void FormMain_DoubleClick(object sender, System.EventArgs e)
			{
				this.WindowState=System.Windows.Forms.FormWindowState.Minimized;
			}
			private void Enviar(string mensajes)
			{
				byte[] Mensaje;
				System.Net.IPEndPoint Server;
				Server = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("230.0.0.1"),20002);
				Mensaje = System.Text.Encoding.Default.GetBytes(mensajes);
				try
				{
					this.UdpOut.Send(Mensaje,Mensaje.Length,Server);
					this.count=0;
				}
				catch(Exception ex)
				{	
					this.statusBarPanelDown.Text=ex.Message;
				}
			}
		}
	}
