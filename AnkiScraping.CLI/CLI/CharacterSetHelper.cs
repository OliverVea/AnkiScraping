namespace AnkiScraping.Host.CLI;

public class CharacterSetHelper
{
    // ReSharper disable UnusedMember.Local
    private const int HiraganaMin = 0x3040;
    private const int HiraganaMax = 0x309F;
    
    private const int KatakanaMin = 0x30A0;
    private const int KatakanaMax = 0x30FF;
    // ReSharper restore UnusedMember.Local
    
    private const int KanjiMin = 0x4E00;
    private const int KanjiMax = 0x9FAF;
    
    public static bool IsKanji(char c)
    {
        return c >= KanjiMin && c <= KanjiMax;
    }
}