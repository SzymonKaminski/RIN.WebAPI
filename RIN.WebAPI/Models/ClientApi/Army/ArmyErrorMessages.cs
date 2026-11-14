namespace RIN.WebAPI.Models.ClientApi
{
    public static class ArmyErrorMessages
    {
        public const string ALREADY_IN_ARMY                    = "You are already a member of an army.";
        public const string NOT_A_MEMBER                       = "You are not a member of this army.";
        public const string NOT_THE_COMMANDER                  = "You are not the commander of this army.";
        public const string NO_PERMISSION_EDIT_RANKS           = "You do not have permission to edit ranks for this army.";
        public const string NO_PERMISSION_APPROVE_APPLICATIONS = "You do not have permission to approve applications for this army.";
        public const string NO_PERMISSION_REJECT_APPLICATIONS  = "You do not have permission to reject applications for this army.";
        public const string INVALID_WEBSITE                    = "The website is not a valid URL.";
    }
}
