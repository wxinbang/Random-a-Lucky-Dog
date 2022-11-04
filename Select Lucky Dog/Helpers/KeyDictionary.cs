using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Select_Lucky_Dog.Helpers
{
    internal static class KeyDictionary
    {
        public enum StringKey:byte
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
            FileSaved
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
