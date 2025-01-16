using BL.Domain;
using BL.Domain.Questions;
using Microsoft.AspNetCore.Identity;

namespace DAL.EF;

public class DataSeeder
{
    private readonly UserManager<IdentityUser> _userManager;

    public DataSeeder(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task Seed(PhygitalDbContext context)
    {
        
        ArgumentNullException.ThrowIfNull(context);
        
        var project = new Project("Phygital", "dit is de interesante beschrijving van de flow");
        context.Add(project);
        var adminUser = await _userManager.FindByNameAsync("admin@kdg.be");
        project.AdminId = adminUser?.Id;
        await context.SaveChangesAsync();
        
        var flows = new List<Flow>
        {
            new(
                1,
                "Betrokkenheid bij het lokale beleid",
                "dit is de interesante beschrijving van de flow",
                project.Id,
                FlowType.LINEAR,
                null,
                [
                    new Flow(
                        1,
                        "Betrokkenheid en participatie",
                        "dit is de interesante beschrijving van de flow",
                        project.Id,
                        FlowType.LINEAR,
                        [
                            new SingleChoiceQuestion(
                                1,
                                "Als jij de begroting van je stad of gemeente zou opmaken, waar zou je dan in de komende jaren vooral op inzetten?",
                                [
                                    new Option("Natuur en ecologie", null),
                                    new Option("Vrije tijd, sport, cultuur",null),
                                    new Option("Huisvesting",null ),
                                    new Option("Onderwijs en kinderopvang",null ),
                                    new Option("Gezondheidszorg en welzijn",null ),
                                    new Option("Verkeersveiligheid en mobiliteit",null ),
                                    new Option("Ondersteunen van lokale handel", null)
                                ],
                                new Media()
                                {
                                    description = "Dit is een video over hoe de bevolking naar de politiek kijkt",
                                    url =
                                        "https://storage.googleapis.com/phygital-public/Questions/informatie_stemming.mp4",
                                    type = MediaType.VIDEO
                                }
                            ),
                            new MultipleChoiceQuestion(
                                2,
                                "Wat zou jou helpen om een keuze te maken tussen de verschillende partijen?",
                                [
                                    new Option()
                                    {
                                        Text = "Meer lessen op school rond de partijprogramma’s", NextQuestionId = null
                                    },
                                    new Option()
                                    {
                                        Text = "Activiteiten in mijn jeugdclub, sportclub… rond de verkiezingen",
                                        NextQuestionId = null
                                    },
                                    new Option()
                                    {
                                        Text = "Een bezoek van de partijen aan mijn school, jeugd/sportclub, …",
                                        NextQuestionId = null
                                    },
                                    new Option()
                                    {
                                        Text = "Een gesprek met mijn ouders rond de gemeentepolitiek",
                                        NextQuestionId = null
                                    },
                                    new Option()
                                    {
                                        Text =
                                            "Een debat georganiseerd door een jeugdhuis met de verschillende partijen",
                                        NextQuestionId = null
                                    }
                                ],
                                new Media()
                                {
                                    description = "Afbeelding van alle partijen",
                                    url =
                                        "https://storage.googleapis.com/phygital-public/Questions/afbeelding_partijen.jpg",
                                    type = MediaType.IMAGE
                                }
                            ),
                            new RangeQuestion(
                                3,
                                "Voel jij je betrokken bij het beleid dat wordt uitgestippeld door je gemeente of stad?",
                                1,
                                5,
                                new Media()
                                {
                                    description =
                                        "Dit is een afbeelding over de betrokkenheid van de bevolking bij de politiek",
                                    url =
                                        "https://storage.googleapis.com/phygital-public/Questions/bevolking_antwerpen.jpg",
                                    type = MediaType.IMAGE
                                })
                        ],
                        null,
                        new Media()
                        {
                            description = "Dit is een afbeelding over de betrokkenheid van jongeren",
                            url =
                                "https://storage.googleapis.com/phygital-public/Questions/OVM-Jongeren-betrekken-bij-de-politiek-hoe-doe-je-dat.jpg",
                            type = MediaType.IMAGE
                        },
                        new DateTime(2024, 1, 1).ToUniversalTime(),
                        new DateTime(2024, 7, 1).ToUniversalTime()
                    ),
                    new Flow(
                        2,
                        "Kiesintenties en participatie aan verkiezingen",
                        "dit is de interesante beschrijving van de flow",
                        project.Id,
                        FlowType.LINEAR,
                        [
                            new SingleChoiceQuestion(
                                1,
                                "Waarop wil jij dat de focus wordt gelegd in het nieuwe stadspark?",
                                [
                                    new Option() { Text = "Sportinfrastructuur", NextQuestionId = null },
                                    new Option() { Text = "Speeltuin voor kinderen", NextQuestionId = null },
                                    new Option() { Text = "Zitbanken en picknickplaatsen", NextQuestionId = null },
                                    new Option() { Text = "Ruimte voor kleine evenementen", NextQuestionId = null },
                                    new Option() { Text = "Drank- en eetmogelijkheden", NextQuestionId = null }
                                ],
                                new Media()
                                {
                                    description = "Video over het nieuwe stadspark in Mechelen",
                                    url = "https://storage.googleapis.com/phygital-public/Questions/video_park.mp4",
                                    type = MediaType.VIDEO
                                }
                            ),
                            new MultipleChoiceQuestion(
                                2,
                                "Jij gaf aan dat je waarschijnlijk niet zal gaan stemmen. Om welke reden(en) zeg je dit?",
                                [
                                    new Option()
                                        { Text = "Ik ben niet geïnteresseerd in politiek", NextQuestionId = null },
                                    new Option()
                                        { Text = "Ik weet niet waar ik moet gaan stemmen", NextQuestionId = null },
                                    new Option()
                                        { Text = "Ik weet niet waarover de verkiezingen gaan", NextQuestionId = null },
                                    new Option()
                                    {
                                        Text = "Ik weet niet wat de verschillende partijen willen doen",
                                        NextQuestionId = null
                                    },
                                    new Option()
                                    {
                                        Text = "Ik weet niet wat de verschillende partijen willen doen",
                                        NextQuestionId = null
                                    }
                                ],
                                new Media()
                                {
                                    description = "Video over hoe het stemmen werkt",
                                    url =
                                        "https://storage.googleapis.com/phygital-public/Questions/hoeWerkthetStemmen.mp3",
                                    type = MediaType.AUDIO
                                }
                            ),
                            new RangeQuestion(
                                3,
                                "Hoe sta jij tegenover deze stelling? “Mijn stad moet meer investeren in fietspaden",
                                1,
                                2,
                                new Media()
                                {
                                    description = "Fietser op een fietspad",
                                    url = "https://storage.googleapis.com/phygital-public/Questions/fietser.jpg",
                                    type = MediaType.IMAGE
                                }
                            ),
                            new OpenQuestion(
                                4,
                                "Je bent schepen van onderwijs voor een dag: waar zet je dan vooral op in?",
                                new Media()
                                {
                                    description = "Afbeelding van de schepen van onderwijs",
                                    url =
                                        "https://storage.googleapis.com/phygital-public/Questions/schepenVanOnderwijs.jpeg",
                                    type = MediaType.IMAGE
                                }
                            )
                        ],
                        null,
                        new Media()
                        {
                            description = "Afbeelding over de kiesintenties en participatie aan verkiezingen",
                            url =
                                "https://storage.googleapis.com/phygital-public/Questions/shutterstock_1937926147_1.jpg",
                            type = MediaType.IMAGE
                        },
                        new DateTime(2024, 1, 1).ToUniversalTime(),
                        new DateTime(2024, 7, 1).ToUniversalTime()
                    )
                ],
                new Media()
                {
                    description = "Afbeelding over de betrokkenheid bij het lokale beleid",
                    url = "https://storage.googleapis.com/phygital-public/Questions/betrekkingBevolking.jpg",
                    type = MediaType.IMAGE
                },
                new DateTime(2024, 1, 1).ToUniversalTime(),
                new DateTime(2024, 7, 1).ToUniversalTime()
            )
        };
        
        project.Flows = flows; 
        await context.SaveChangesAsync();
    }
}