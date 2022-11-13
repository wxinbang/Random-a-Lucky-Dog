using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Select_Lucky_Dog.Helpers
{
    internal static class KeyDictionary
    {
        internal enum StringKey : byte
        {
            AllAlreadyFinished,
            Close,
            CheckMarkConfirmText,
            CheckMarkTitle,
            CheckMarkContent,
            PraiseTitle,
            PraiseContent,
            PraiseClose,
            JoinProgramTitle,
            JoinProgramContent,
            JoinProgramClose,
            JoinProgramPrimary,
            ExceptionTitle,
            SendEmail,
            FirstRun_BodyText,
            FirstRunDialogPrimaryButtonText,
            FirstRunDialogTitle,
            FileSaved,
            FileNotFind,
            FeatureOfferedInTheNextVersion,
            DeleteFinished,
            Feedback,
            ExceptionAt, 
            SoftwareCrashes,
            UserName,
            Password,
            ExistNullValue,
            InsertUSBDrive,
            ExportIdentityFile,
            Export,
            Cancel,
            NoRequiredPermissions,
            VerifyPassword,
            Verify,

        }
        internal enum SettingKey : byte
        {
            FileName,
            Mark,
            Saved,
            JoinProgram,
        }
    }
}
