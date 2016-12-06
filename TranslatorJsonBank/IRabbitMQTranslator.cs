namespace TranslatorJsonBank
{
    public interface IRabbitMQTranslator
    {
        string[] Translate(string RecivedFormat);
    }
}