namespace TranslatorWebserviceBank
{
    public interface IRabbitMQTranslator
    {
        string[] Translate(string RecivedFormat);
    }
}