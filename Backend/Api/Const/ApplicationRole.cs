namespace Api.Const
{
    public static class ApplicationRole
    {
        public static string ProductManager => "ProductManager";
        public static string InventoryManage => "InventoryManager";
        public static string BranchStaff => "BranchStaff";
        public static string BranchManager => "BranchManager";

        public static string[] All => [ProductManager, InventoryManage, BranchManager, BranchStaff];
    }
}
