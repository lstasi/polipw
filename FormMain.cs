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
						Index2=IN.IndexOf(" ",Index1);
						if(Index2>0 && Index2!=Index1)
						{
							Index1++; 
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
			else if(IN.CompareTo("Report")==0)//If recive a report message rise onnewData
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
				
				System.Drawing.Rectangle ScreenBound=new System.Drawing.Rectangle();
				this.Location.X=0;
				this.Location.Y=0;
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
				this.progressBarSpeed = new System.Windows.Forms.ProgressBar();
				this.pictureBoxMark = new System.Windows.Forms.PictureBox();
				this.timerSocket = new System.Windows.Forms.Timer(this.components);
				this.statusBarStatus = new System.Windows.Forms.StatusBar();
				this.statusBarPanelDown = new System.Windows.Forms.StatusBarPanel();
				this.statusBarPanelStatus = new System.Windows.Forms.StatusBarPanel();
				((System.ComponentModel.ISupportInitialize)(this.statusBarPanelDown)).BeginInit();
				((System.ComponentModel.ISupportInitialize)(this.statusBarPanelStatus)).BeginInit();
				this.SuspendLayout();
				// 
				// progressBarSpeed
				// 
				this.progressBarSpeed.Location = new System.Drawing.Point(6, 12);
				this.progressBarSpeed.Name = "progressBarSpeed";
				this.progressBarSpeed.Size = new System.Drawing.Size(112, 8);
				this.progressBarSpeed.TabIndex = 2;
				// 
				// pictureBoxMark
				// 
				this.pictureBoxMark.BackColor = System.Drawing.Color.Red;
				this.pictureBoxMark.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
				this.pictureBoxMark.Location = new System.Drawing.Point(6, 6);
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
				this.statusBarStatus.Location = new System.Drawing.Point(0, 30);
				this.statusBarStatus.Name = "statusBarStatus";
				this.statusBarStatus.Panels.AddRange(new System.Windows.Forms.StatusBarPanel[] {
																								   this.statusBarPanelDown,
																								   this.statusBarPanelStatus});
				this.statusBarStatus.ShowPanels = true;
				this.statusBarStatus.Size = new System.Drawing.Size(122, 16);
				this.statusBarStatus.SizingGrip = false;
				this.statusBarStatus.TabIndex = 4;
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
				// FormMain
				// 
				this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
				this.ClientSize = new System.Drawing.Size(122, 46);
				this.Controls.AddRange(new System.Windows.Forms.Control[] {
																			  this.statusBarStatus,
																			  this.progressBarSpeed,
																			  this.pictureBoxMark});
				this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
				this.MaximizeBox = false;
				this.Name = "FormMain";
				this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
				this.Text = "Pol-IP";
				this.TopMost = true;
				this.Closing += new System.ComponentModel.CancelEventHandler(this.FormMain_Closing);
				this.Load += new System.EventHandler(this.FormMain_Load);
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
				if(this.BufferINSocket.Report)//If report is true send start to the server
				{
					byte[] Mensaje;
					System.Net.IPEndPoint Server;
					Server = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("230.0.0.1"),20002);
					Mensaje = System.Text.Encoding.Default.GetBytes("Start");
					try
					{
						this.UdpOut.Send(Mensaje,Mensaje.Length,Server);
					}
					catch(Exception ex)
					{	
						this.statusBarPanelDown.Text=ex.Message;
					}
				}
				else
				{

					float FSpeed;
					this.count=0;//Reset the counter of no data in
					
					this.progressBarSpeed.Maximum=this.BufferINSocket.LinkSpeed;
					this.progressBarSpeed.Value=this.BufferINSocket.ActualSpeed;
					//Calculate the place of the mark in the progressbar
					this.pictureBoxMark.Left=this.progressBarSpeed.Left
						+ ((this.BufferINSocket.AsingSpeed * this.progressBarSpeed.Width)
						/this.BufferINSocket.LinkSpeed);
					
					FSpeed=System.Convert.ToSingle(this.BufferINSocket.ActualSpeed);
					FSpeed=FSpeed/1024;
					this.statusBarPanelDown.Text=FSpeed.ToString("F") + " Kb/s";
					if(this.WindowState==0)//If the windows is minimize send text to title
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
					}
					else
					{
						if(count==0)//If 0 means data arrived so clean errors
							this.statusBarPanelStatus.Text="";
						else if(count==1)//One time say no data warning
							this.statusBarPanelStatus.Text="No Data";
						else//More times draw points
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
				}
			}

			private void FormMain_Load(object sender, System.EventArgs e)
			{
				//Send start message to server
				byte[] Mensaje;
				System.Net.IPEndPoint Server;
				Server = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("230.0.0.1"),20002);
				Mensaje = System.Text.Encoding.Default.GetBytes("Start");
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

			private void FormMain_Closing(object sender, System.ComponentModel.CancelEventArgs e)
			{
				//Send stop message on exit
				byte[] Mensaje;
				System.Net.IPEndPoint Server;
				Server = new System.Net.IPEndPoint(System.Net.IPAddress.Parse("230.0.0.1"),20002);
				Mensaje = System.Text.Encoding.Default.GetBytes("Stop");
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
