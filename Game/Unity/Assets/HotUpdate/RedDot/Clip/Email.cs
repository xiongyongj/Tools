
using System.Collections.Generic;

public class EmailProps : RedPointBaseProps {
    public long UID;
}

[RedPoint(100110)]
public class Email : RedPointBase<EmailProps> {
    public override void GenerateCells() {
        _cells.Clear();
        // List<MailProtoInfo> list = CatchGhostEmailSystem.GetData();
        // for (int i = 0; i < list.Count; ++i) {
        //     MailProtoInfo info = list[i];
        //     AddCell(info.EmailId);
        //     AddUpdateFunc2Cell(info.EmailId, () => new EmailProps() {
        //         UID = info.EmailId
        //     });
        // }
    }

    // protected override bool _isVisible(long target) {
    //     EmailProps props = UpdateProps(target);
    //     if (props == null) {
    //         return false;
    //     }
    //
    //     MailProtoInfo info = EmailSystem.Get(props.UID);
    //     if (info == null) {
    //         return false;
    //     }
    //     return info.ItemIds.Count > 0 && !info.HadReceived;
    // }
}
