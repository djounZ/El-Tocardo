using Microsoft.EntityFrameworkCore;
using ElTocardo.Infrastructure.Mediator.EntityFramework.Configurations;

namespace ElTocardo.Infrastructure.Mediator.EntityFramework.Data.ModelBuilderExtensions
{
    public static class PresetChatInstructionModelBuilderExtensions
    {
        public static void BuildPresetChatInstruction(this ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new PresetChatInstructionConfiguration());
        }
    }
}
