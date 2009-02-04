using System;
using System.Collections.Generic;
using System.Text;

namespace Duplicati.GUI.Wizard_pages
{
    /// <summary>
    /// This class wraps all settings avalible in the wizard pages
    /// </summary>
    public class WizardSettingsWrapper
    {
        private Dictionary<string, object> m_settings;
        private const string PREFIX = "WSW_";

        public enum MainAction
        {
            Unknown,
            Add,
            Edit,
            Restore,
            Remove,
            RunNow
        };

        public enum BackendType
        {
            File,
            FTP,
            SSH,
            S3,
            WebDav,
            Unknown
        };

        public WizardSettingsWrapper(Dictionary<string, object> settings)
        {
            m_settings = settings;
        }

        /// <summary>
        /// Clears all settings, and makes the setting object reflect the schedule
        /// </summary>
        /// <param name="schedule">The schedule to reflect</param>
        public void ReflectSchedule(Datamodel.Schedule schedule)
        {
            MainAction action = this.PrimayAction;
            m_settings.Clear();

            this.ScheduleID = schedule.ID;
            this.ScheduleName = schedule.Name;
            this.SchedulePath = schedule.Path;
            this.SourcePath = schedule.Tasks[0].SourcePath;
            this.SourceFilter = schedule.Tasks[0].EncodedFilter;
            this.BackupPassword = schedule.Tasks[0].Encryptionkey;

            switch (schedule.Tasks[0].Service.ToLower())
            {
                case "file":
                    Datamodel.Backends.File file = new Datamodel.Backends.File(schedule.Tasks[0]);
                    this.FileSettings.Username = file.Username;
                    this.FileSettings.Password = file.Password;
                    this.FileSettings.Path = file.DestinationFolder;
                    this.Backend = BackendType.File;
                    break;
                case "ftp":
                    Datamodel.Backends.FTP ftp = new Duplicati.Datamodel.Backends.FTP(schedule.Tasks[0]);
                    this.FTPSettings.Username = ftp.Username;
                    this.FTPSettings.Password = ftp.Password;
                    this.FTPSettings.Path = ftp.Folder;
                    this.FTPSettings.Server = ftp.Host;
                    this.FTPSettings.Port = ftp.Port;
                    this.Backend = BackendType.FTP;
                    break;
                case "ssh":
                    Datamodel.Backends.SSH ssh = new Duplicati.Datamodel.Backends.SSH(schedule.Tasks[0]);
                    this.SSHSettings.Username = ssh.Username;
                    this.SSHSettings.Password = ssh.Password;
                    this.SSHSettings.Path = ssh.Folder;
                    this.SSHSettings.Server = ssh.Host;
                    this.SSHSettings.Port = ssh.Port;
                    this.SSHSettings.Passwordless = ssh.Passwordless;
                    this.Backend = BackendType.SSH;
                    break;
                case "s3":
                    Datamodel.Backends.S3 s3 = new Duplicati.Datamodel.Backends.S3(schedule.Tasks[0]);
                    this.S3Settings.Username = s3.AccessID;
                    this.S3Settings.Password = s3.AccessKey;
                    this.S3Settings.UseEuroServer = s3.UseEuroBucket;
                    this.S3Settings.UseSubDomains = s3.UseSubdomainStrategy;
                    this.S3Settings.Path = s3.BucketName;
                    if (!string.IsNullOrEmpty(s3.Prefix))
                        this.S3Settings.Path += "/" + s3.Prefix;
                    this.Backend = BackendType.S3;
                    break;
                case "webdav":
                    this.Backend = BackendType.WebDav;
                    break;
            }

            this.BackupTimeOffset = schedule.When;
            this.RepeatInterval = schedule.Repeat;
            this.FullBackupInterval = schedule.FullAfter;
            this.MaxFullBackups = (int)schedule.KeepFull;
            this.BackupExpireInterval = schedule.KeepTime;
            this.UploadSpeedLimit = schedule.UploadBandwidth;
            this.DownloadSpeedLimit = schedule.DownloadBandwidth;
            this.BackupSizeLimit = schedule.MaxUploadsize;
            this.VolumeSize = schedule.VolumeSize;
            //this.ThreadPriority = schedule.ThreadPriority;
            //this.AsyncTransfer = schedule.AsyncTransfer;
            this.EncodedFilters = schedule.Tasks[0].EncodedFilter;
            
            this.PrimayAction = action;
        }


        /// <summary>
        /// Writes all values from the session object back into a schedule object
        /// </summary>
        /// <param name="schedule"></param>
        public void UpdateSchedule(Datamodel.Schedule schedule)
        {
            schedule.Name = this.ScheduleName;
            schedule.Path = this.SchedulePath;
            schedule.Tasks[0].SourcePath = this.SourcePath;
            schedule.Tasks[0].EncodedFilter = this.SourceFilter;
            schedule.Tasks[0].Encryptionkey = this.BackupPassword;

            switch (this.Backend)
            {
                case BackendType.File:
                    Datamodel.Backends.File file = new Datamodel.Backends.File(schedule.Tasks[0]);
                    file.Username = this.FileSettings.Username;
                    file.Password = this.FileSettings.Password;
                    file.DestinationFolder = this.FileSettings.Path;
                    break;
                case BackendType.FTP:
                    Datamodel.Backends.FTP ftp = new Duplicati.Datamodel.Backends.FTP(schedule.Tasks[0]);
                    ftp.Username = this.FTPSettings.Username;
                    ftp.Password = this.FTPSettings.Password;
                    ftp.Folder = this.FTPSettings.Path;
                    ftp.Host = this.FTPSettings.Server;
                    ftp.Port = this.FTPSettings.Port;
                    break;
                case BackendType.SSH:
                    Datamodel.Backends.SSH ssh = new Duplicati.Datamodel.Backends.SSH(schedule.Tasks[0]);
                    ssh.Username = this.SSHSettings.Username;
                    ssh.Password = this.SSHSettings.Password;
                    ssh.Folder = this.SSHSettings.Path;
                    ssh.Host = this.SSHSettings.Server;
                    ssh.Port = this.SSHSettings.Port;
                    ssh.Passwordless = this.SSHSettings.Passwordless;
                    break;
                case BackendType.S3:
                    Datamodel.Backends.S3 s3 = new Duplicati.Datamodel.Backends.S3(schedule.Tasks[0]);
                    s3.AccessID = this.S3Settings.Username;
                    s3.AccessKey = this.S3Settings.Password;
                    s3.UseEuroBucket = this.S3Settings.UseEuroServer;
                    s3.UseSubdomainStrategy = this.S3Settings.UseSubDomains;
                    if (this.S3Settings.Path.Contains("/"))
                    {
                        int index = this.S3Settings.Path.IndexOf("/");
                        s3.BucketName = this.S3Settings.Path.Substring(0, index);
                        s3.Prefix = this.S3Settings.Path.Substring(index + 1);
                    }
                    else
                    {
                        s3.BucketName = this.S3Settings.Path;
                        s3.Prefix = "";
                    }
                    break;
                case BackendType.WebDav:
                    break;
            }

            schedule.When = this.BackupTimeOffset;
            schedule.Repeat = this.RepeatInterval;
            schedule.FullAfter = this.FullBackupInterval;
            schedule.KeepFull = this.MaxFullBackups;
            
            schedule.KeepTime = this.BackupExpireInterval;
            schedule.UploadBandwidth = this.UploadSpeedLimit;
            schedule.DownloadBandwidth = this.DownloadSpeedLimit;
            schedule.MaxUploadsize = this.BackupSizeLimit;

            schedule.VolumeSize = this.VolumeSize;
            //schedule.ThreadPriority = this.ThreadPriority;
            //schedule.AsyncTransfer = this.AsyncTransfer;
            schedule.Tasks[0].EncodedFilter = this.EncodedFilters;
        }

        /// <summary>
        /// Internal helper to typecast the values, and protect agains missing values
        /// </summary>
        /// <typeparam name="T">The type of the value stored</typeparam>
        /// <param name="key">The key used to identify the setting</param>
        /// <param name="default">The value to use if there is no value stored</param>
        /// <returns>The value or the default value</returns>
        public T GetItem<T>(string key, T @default)
        {
            return m_settings.ContainsKey(PREFIX + key) ? (T)m_settings[PREFIX + key] : @default;
        }

        public void SetItem(string key, object value)
        {
            m_settings[PREFIX + key] = value;
        }

        /// <summary>
        /// The action taken on the primary page
        /// </summary>
        public MainAction PrimayAction
        {
            get { return GetItem<MainAction>("PrimaryAction", MainAction.Unknown); }
            set { SetItem("PrimaryAction", value); }
        }

        /// <summary>
        /// The ID of the schedule being edited, if any
        /// </summary>
        public long ScheduleID
        {
            get { return GetItem<long>("ScheduleID", 0); }
            set { SetItem("ScheduleID", value); }
        }

        /// <summary>
        /// The name assigned to the backup
        /// </summary>
        public string ScheduleName
        {
            get { return GetItem<string>("ScheduleName", ""); }
            set { SetItem("ScheduleName", value); }
        }

        /// <summary>
        /// The group path of the backup
        /// </summary>
        public string SchedulePath
        {
            get { return GetItem<string>("SchedulePath", ""); }
            set { SetItem("SchedulePath", value); }
        }

        /// <summary>
        /// The path of the files to be backed up
        /// </summary>
        public string SourcePath
        {
            get { return GetItem<string>("SourcePath", ""); }
            set { SetItem("SourcePath", value); }
        }

        /// <summary>
        /// The encoded filter string 
        /// </summary>
        public string SourceFilter
        {
            get { return GetItem<string>("SourceFilter", ""); }
            set { SetItem("SourceFilter", value); }
        }

        /// <summary>
        /// The password that protects the backup
        /// </summary>
        public string BackupPassword
        {
            get { return GetItem<string>("BackupPassword", ""); }
            set { SetItem("BackupPassword", value); }
        }

        /// <summary>
        /// The currently active backend type
        /// </summary>
        public BackendType Backend
        {
            get { return GetItem<BackendType>("Backend", BackendType.Unknown); }
            set { SetItem("Backend", value); }
        }

        /// <summary>
        /// Returns a customized settings object describing settings for a file-based backend
        /// </summary>
        public FileSettings FileSettings { get { return new FileSettings(this); } }

        /// <summary>
        /// Returns a customized settings object describing settings for a ssh-based backend
        /// </summary>
        public SSHSettings SSHSettings { get { return new SSHSettings(this); } }

        /// <summary>
        /// Returns a customized settings object describing settings for a ftp-based backend
        /// </summary>
        public FTPSettings FTPSettings { get { return new FTPSettings(this); } }

        /// <summary>
        /// Returns a customized settings object describing settings for a S3-based backend
        /// </summary>
        public S3Settings S3Settings { get { return new S3Settings(this); } }

        /// <summary>
        /// The offset for running backups
        /// </summary>
        public DateTime BackupTimeOffset
        {
            get { return GetItem<DateTime>("BackupTimeOffset", DateTime.Now); }
            set { SetItem("BackupTimeOffset", value); }
        }

        /// <summary>
        /// The interval at which to repeat the backup
        /// </summary>
        public string RepeatInterval
        {
            get { return GetItem<string>("RepeatInterval", ""); }
            set { SetItem("RepeatInterval", value); }
        }

        /// <summary>
        /// The interval at which to perform full backups
        /// </summary>
        public string FullBackupInterval
        {
            get { return GetItem<string>("FullBackupInterval", ""); }
            set { SetItem("FullBackupInterval", value); }
        }

        /// <summary>
        /// The number om full backups to keep
        /// </summary>
        public int MaxFullBackups
        {
            get { return GetItem<int>("MaxFullBackups", 0); }
            set { SetItem("MaxFullBackups", value); }
        }

        /// <summary>
        /// The interval after which backups are deleted
        /// </summary>
        public string BackupExpireInterval
        {
            get { return GetItem<string>("BackupExpireInterval", ""); }
            set { SetItem("BackupExpireInterval", value); }
        }


        /// <summary>
        /// The interval at which to perform full backups
        /// </summary>
        public string UploadSpeedLimit
        {
            get { return GetItem<string>("UploadSpeedLimit", ""); }
            set { SetItem("UploadSpeedLimit", value); }
        }

        /// <summary>
        /// The interval at which to perform full backups
        /// </summary>
        public string DownloadSpeedLimit
        {
            get { return GetItem<string>("DownloadSpeedLimit", ""); }
            set { SetItem("DownloadSpeedLimit", value); }
        }

        /// <summary>
        /// The max size the set of backup files may occupy
        /// </summary>
        public string BackupSizeLimit
        {
            get { return GetItem<string>("BackupSizeLimit", ""); }
            set { SetItem("BackupSizeLimit", value); }
        }

        /// <summary>
        /// The size of each volume in the backup set
        /// </summary>
        public string VolumeSize
        {
            get { return GetItem<string>("VolumeSize", ""); }
            set { SetItem("VolumeSize", value); }
        }

        /// <summary>
        /// The size of each volume in the backup set
        /// </summary>
        public string ThreadPriority
        {
            get { return GetItem<string>("ThreadPriority", ""); }
            set { SetItem("ThreadPriority", value); }
        }

        /// <summary>
        /// The size of each volume in the backup set
        /// </summary>
        public bool AsyncTransfer
        {
            get { return GetItem<bool>("AsyncTransfer", false); }
            set { SetItem("AsyncTransfer", value); }
        }

        /// <summary>
        /// The size of each volume in the backup set
        /// </summary>
        public string EncodedFilters
        {
            get { return GetItem<string>("EncodedFilters", ""); }
            set { SetItem("EncodedFilters", value); }
        }

        /// <summary>
        /// A value indicating if the created/edited backup should run immediately
        /// </summary>
        public bool RunImmediately
        {
            get { return GetItem<bool>("RunImmediately", false); }
            set { SetItem("RunImmediately", value); }
        }

        /// <summary>
        /// A value indicating if the backup should be forced full
        /// </summary>
        public bool ForceFull
        {
            get { return GetItem<bool>("ForceFull", false); }
            set { SetItem("ForceFull", value); }
        }

        /// <summary>
        /// A value indicating the backup to restore
        /// </summary>
        public DateTime RestoreTime
        {
            get { return GetItem<DateTime>("RestoreTime", new DateTime()); }
            set { SetItem("RestoreTime", value); }
        }

        /// <summary>
        /// A value indicating where to place the restored files
        /// </summary>
        public string RestorePath
        {
            get { return GetItem<string>("RestorePath", ""); }
            set { SetItem("RestorePath", value); }
        }

        /// <summary>
        /// A value indicating the filter applied to the restored files
        /// </summary>
        public string RestoreFilter
        {
            get { return GetItem<string>("RestoreFilter", ""); }
            set { SetItem("RestoreFilter", value); }
        }

    }

    /// <summary>
    /// Class that represents the settings for a backend
    /// </summary>
    public class BackendSettings
    {
        protected WizardSettingsWrapper m_parent;

        public BackendSettings(WizardSettingsWrapper parent)
        {
            m_parent = parent;
        }

        /// <summary>
        /// The username used to authenticate towards the remote path
        /// </summary>
        public string Username
        {
            get { return m_parent.GetItem<string>("Backend:Username", ""); }
            set { m_parent.SetItem("Backend:Username", value); }
        }

        /// <summary>
        /// The password used to authenticate towards the remote path
        /// </summary>
        public string Password
        {
            get { return m_parent.GetItem<string>("Backend:Password", ""); }
            set { m_parent.SetItem("Backend:Password", value); }
        }

        /// <summary>
        /// The path used on the server
        /// </summary>
        public string Path
        {
            get { return m_parent.GetItem<string>("Backend:Path", ""); }
            set { m_parent.SetItem("Backend:Path", value); }
        }
    }


    /// <summary>
    /// Class that represents the settings for a file backend
    /// </summary>
    public class FileSettings : BackendSettings
    {
        public FileSettings(WizardSettingsWrapper parent)
            : base(parent)
        {
        }

    }


    /// <summary>
    /// Class that represents the settings for a web based backend
    /// </summary>
    public class WebSettings : BackendSettings
    {
        protected int m_defaultPort = 0;

        public WebSettings(WizardSettingsWrapper parent)
            : base(parent)
        {
        }

        /// <summary>
        /// The hostname of the server
        /// </summary>
        public string Server
        {
            get { return m_parent.GetItem<string>("WEB:Server", ""); }
            set { m_parent.SetItem("WEB:Server", value); }
        }

        /// <summary>
        /// The port used to communicate with the server
        /// </summary>
        public int Port
        {
            get { return m_parent.GetItem<int>("WEB:Port", m_defaultPort); }
            set { m_parent.SetItem("WEB:Port", value); }
        }

    }

    /// <summary>
    /// Class that represents the settings for a ftp backend
    /// </summary>
    public class FTPSettings : WebSettings
    {
        public FTPSettings(WizardSettingsWrapper parent)
            : base(parent)
        {
            m_defaultPort = 21;
        }

    }

    /// <summary>
    /// Class that represents the settings for a ssh backend
    /// </summary>
    public class SSHSettings : WebSettings
    {
        public SSHSettings(WizardSettingsWrapper parent)
            : base(parent)
        {
            m_defaultPort = 22;
        }

        /// <summary>
        /// A value indiciating if the connection is passwordless
        /// </summary>
        public bool Passwordless
        {
            get { return m_parent.GetItem<bool>("SSH:Passwordless", false); }
            set { m_parent.SetItem("SSH:Passwordless", value); }
        }
    }

    /// <summary>
    /// Class that represents the settings for a ssh backend
    /// </summary>
    public class S3Settings : BackendSettings
    {
        public S3Settings(WizardSettingsWrapper parent)
            : base(parent)
        {
        }

        /// <summary>
        /// A value indicating if the server should be placed in europe
        /// </summary>
        public bool UseEuroServer
        {
            get { return m_parent.GetItem<bool>("S3:UseEuroServer", false); }
            set { m_parent.SetItem("S3:UseEuroServer", value); }
        }

        /// <summary>
        /// A value indicating if the connection should use subdomain access
        /// </summary>
        public bool UseSubDomains
        {
            get { return m_parent.GetItem<bool>("S3:UseSubDomains", false); }
            set { m_parent.SetItem("S3:UseSubDomains", value); }
        }

    }


}