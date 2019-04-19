public static class Constants
{
    public const int Client_SetDestination = 1;
    public const int Client_Tacle = 2;
    public const int Client_Message = 3;
    public const int Client_RequestHack = 4;
    public const int Client_SetHack = 5;
    public const int Client_ConnectionId = 6;
    public const int Client_Open_Door = 7;
    public const int Client_StartTaking = 8;
    public const int Client_StartGiving = 9;

    public const int Server_MoveCharacter = 42;
    public const int Server_CreateCharacter = 43;
    public const int Server_Message = 44;
    public const int Server_GetHack = 45;
    public const int Server_Snapshot = 50;
    public const int Server_UpdateObject = 57;
    public const int Server_SnapshotEnd = 9999;
    public const int Server_SetConnectionId = 58;

    public const int Client_Lobby_ConnectionId = 101;
    public const int Client_Lobby_SetTeam = 102;
    public const int Client_Lobby_Ready = 103;
    public const int Client_Lobby_Cancel = 104;

    public const int Server_Lobby_SetConnectionId = 151;
    public const int Server_Lobby_LobbyState = 152;
    public const int Server_Lobby_StartGame = 153;


    public static readonly string[] LobbyNames = new string[] {"Le Redoutable", "Le Triomphant", "Le Téméraire", "Le Vigilant", "Le Terrible",
        "Le Conquérant", "Le Héros", "Le Glorieux", "Le Tonnant", "Le Foudroyant", "L'Indomptable", "L'Inflexible"};
}
