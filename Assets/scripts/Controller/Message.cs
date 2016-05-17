
namespace dassault
{
    /// <summary>
    /// Visitor that handle received messages.
    /// </summary>
    public interface IMessageVisitor
    {
        // Internal messages
        void HandleMessage(ReceptionError msg);
        void HandleMessage(ConnectTo msg);
        void HandleMessage(SendAnnotation msg);
        void HandleMessage(SendCurrentStep msg);
        void HandleMessage(SendAnnotationAck msg);
        void HandleMessage(CommentVisibility msg);
        void HandleMessage(ToolVisibility msg);
        void HandleMessage(ReferenceVisibility msg);
        void HandleMessage(AnnotationVisibility msg);
        void HandleMessage(LocalizationVisibility msg);
        // Bluetooth messages
        void HandleMessage(ScenarioCmd cmd);
        void HandleMessage(NextStepCmd cmd);
        void HandleMessage(PreviousStepCmd cmd);
        void HandleMessage(BookmarksCmd cmd);
        void HandleMessage(CurrentViewNextCmd cmd);
        void HandleMessage(CurrentViewPreviousCmd cmd);
        void HandleMessage(SwitchLocalizationCmd cmd);
        void HandleMessage(SwitchReferencesCmd cmd);
        void HandleMessage(SwitchToolsCmd cmd);
        void HandleMessage(SwitchCommentsCmd cmd);
        void HandleMessage(SwitchAnnotationsCmd cmd);
        void HandleMessage(SwitchGUICmd cmd);
        void HandleMessage(WatchScreenChangedCmd cmd);
        void HandleMessage(WatchStepPathChangedCmd cmd);
        void HandleMessage(TakeScreenshotCmd cmd);
        void HandleMessage(DiagnosticCmd cmd);
        void HandleMessage(ConnectionCmd cmd);
        void HandleMessage(DisconnectionCmd cmd);
        void HandleMessage(ConnectedStatus sts);
        void HandleMessage(AskReSynchronizationCmd cmd);
        void HandleMessage(ReSynchronizationCmd cmd);
        void HandleMessage(ConnectionRefusedStatus sts);
        void HandleMessage(AnnotationCmd cmd);
        void HandleMessage(CaptureCmd cmd);
        void HandleMessage(ScenariiStatusCmd cmd);
        void HandleMessage(ShowNewProcedureCmd cmd);
        void HandleMessage(ConnectionToSupportCmd cmd);
        void HandleMessage(DisconnectionToSupportCmd cmd);
        void HandleMessage(SupportStatusCmd cmd);
        void HandleMessage(StreamParametersCmd cmd);
        void HandleMessage(ConnectedToStreamCmd cmd);
        void HandleMessage(DownloadProcedureFromURLCmd cmd);
    }

    /// <summary>
    /// Base class of all messages.
    /// </summary>
    public abstract class Message
    {
        public abstract void AcceptVisitor(IMessageVisitor visitor);
    }

    /// <summary>
    /// Message sent by a reception thread to the main thread when a reception error occur.
    /// </summary>
    public class ReceptionError : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public ReceptionError(int serverId, int clientId, int errorCode)
        {
            ServerId = serverId;
            ClientId = clientId;
            ErrorCode = errorCode;
        }

        public readonly int ServerId;
        public readonly int ClientId;
        public readonly int ErrorCode;
    }

    public class ConnectTo : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public ConnectTo(string deviceName, string deviceAddress)
        {
            DeviceName = deviceName;
            DeviceAddress = deviceAddress;
        }

        public readonly string DeviceName;
        public readonly string DeviceAddress;
    }

    public class SendAnnotation : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public SendAnnotation(CaptureCmd cmd)
        {
            Cmd = cmd;
        }

        public readonly CaptureCmd Cmd;
    }

    public class SendCurrentStep : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public SendCurrentStep(WatchStepPathChangedCmd cmd)
        {
            Cmd = cmd;
        }

        public readonly WatchStepPathChangedCmd Cmd;
    }

    public class SendAnnotationAck : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public SendAnnotationAck(AnnotationCmd cmd)
        {
            Cmd = cmd;
        }

        public readonly AnnotationCmd Cmd;
    }

    public abstract class VisibilityChanged : Message
    {
        public VisibilityChanged(bool visible)
        {
            Visible = visible;
        }

        public readonly bool Visible;
    }

    public class CommentVisibility : VisibilityChanged
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public CommentVisibility(bool visible)
            : base(visible)
        {
        }
    }

    public class ToolVisibility : VisibilityChanged
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public ToolVisibility(bool visible)
            : base(visible)
        {
        }
    }

    public class ReferenceVisibility : VisibilityChanged
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public ReferenceVisibility(bool visible)
            : base(visible)
        {
        }
    }

    public class AnnotationVisibility : VisibilityChanged
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public AnnotationVisibility(bool visible)
            : base(visible)
        {
        }
    }

    public class LocalizationVisibility : VisibilityChanged
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public LocalizationVisibility(bool visible)
            : base(visible)
        {
        }
    }

    public class ScenarioCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public ScenarioCmd(string projectName)
        {
            ProjectName = projectName;
        }

        public readonly string ProjectName;
    }

    public class NextStepCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }
    }

    public class PreviousStepCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }
    }

    public class BookmarksCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public BookmarksCmd(int bookmarkIdx)
        {
            BookmarkIdx = bookmarkIdx;
        }

        public readonly int BookmarkIdx;
    }

    public class CurrentViewNextCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }
    }

    public class CurrentViewPreviousCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }
    }

    public class SwitchLocalizationCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }
    }

    public class SwitchReferencesCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }
    }

    public class SwitchToolsCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }
    }

    public class SwitchCommentsCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }
    }

    public class SwitchAnnotationsCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }
    }

    public class SwitchGUICmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }
    }

    public class WatchScreenChangedCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public WatchScreenChangedCmd(int screenIndex)
        {
            ScreenIdx = screenIndex;
        }

        public readonly int ScreenIdx;
    }

    public class WatchStepPathChangedCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public WatchStepPathChangedCmd(string stepPath)
        {
            StepPath = stepPath;
        }

        public readonly string StepPath;
    }

    public class TakeScreenshotCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }
    }

    public class DiagnosticCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public DiagnosticCmd(string stepPath, bool accept)
        {
            StepPath = stepPath;
            Accept = accept;
        }

        public readonly string StepPath;
        public readonly bool Accept;
    }

    public class ConnectionCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public ConnectionCmd(string deviceName, string deviceAddr, bool connectWithTab, int connectionId)
        {
            DeviceName = deviceName;
            DeviceAddr = deviceAddr;
            ConnectWithTab = connectWithTab;
            ConnectionId = connectionId;
        }

        public readonly string DeviceName;
        public readonly string DeviceAddr;
        public readonly bool ConnectWithTab;
        public readonly int ConnectionId;
    }

    public class DisconnectionCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public DisconnectionCmd(int connectionId)
        {
            ConnectionId = connectionId;
        }

        public readonly int ConnectionId;
    }

    public class ConnectedStatus : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public ConnectedStatus(int connectionId)
        {
            ConnectionId = connectionId;
        }

        public readonly int ConnectionId;
    }

    public class AskReSynchronizationCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }
    }

    public class ReSynchronizationCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public ReSynchronizationCmd(ScenarioState state)
        {
            State = state;
        }

        public readonly ScenarioState State;
    }

    public class ConnectionRefusedStatus : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public ConnectionRefusedStatus(int connectionId)
        {
            ConnectionId = connectionId;
        }

        public readonly int ConnectionId;
    }

    public class AnnotationCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public AnnotationCmd(string stepPath, byte[] imageContent)
        {
            StepPath = stepPath;
            ImageContent = imageContent;
        }

        public readonly string StepPath;
        public readonly byte[] ImageContent;
    }

    public class CaptureCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public CaptureCmd(string stepPath, byte[] image)
        {
            StepPath = stepPath;
            Image = image;
        }

        public readonly string StepPath;
        public readonly byte[] Image;
    }

    public class ScenariiStatusCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public ScenariiStatusCmd(string[] scenariiList)
        {
            m_scenariiList = scenariiList;
        }

        public readonly string[] m_scenariiList;
    }

    public class ShowNewProcedureCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public ShowNewProcedureCmd(bool status, string procedureName)
        {
            m_status = status;
            m_procedureName = procedureName;
        }

        public readonly bool m_status;
        public readonly string m_procedureName;
    }

    public class ConnectionToSupportCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public ConnectionToSupportCmd(ConnectionType type, string address, string expertNickname)
        {
            m_type = type;
            m_address = address;
            m_expertNickname = expertNickname;
        }

        public readonly ConnectionType m_type;
        public readonly string m_address;
        public readonly string m_expertNickname;
    }


    public class DisconnectionToSupportCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }
        public DisconnectionToSupportCmd()
        {
            // no payload data.
        }
    }


    public class SupportStatusCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public SupportStatusCmd(bool accepted)
        {
            m_accepted = accepted ;
        }

        public readonly bool m_accepted;

    }

    public class StreamParametersCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public StreamParametersCmd(string url, int port)
        {
            m_url = url;
            m_port = port;
        }

        public readonly string m_url;
        public readonly int m_port;

    }

    public class ConnectedToStreamCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public ConnectedToStreamCmd(bool connected)
        {
            m_connected = connected;
        }

        public readonly bool m_connected;

    }

    public class DownloadProcedureFromURLCmd : Message
    {
        public override void AcceptVisitor(IMessageVisitor visitor)
        {
            visitor.HandleMessage(this);
        }

        public DownloadProcedureFromURLCmd(string name, string url)
        {
            m_name = name; 
            m_url = url;
        }

        public readonly string m_name;
        public readonly string m_url;
    }
}
