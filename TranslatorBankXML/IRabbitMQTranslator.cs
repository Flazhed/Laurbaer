namespace TranslatorBankXML
{
    public interface IRabbitMQTranslator
    {
        string[] Translate(string RecivedFormat);
    }
}