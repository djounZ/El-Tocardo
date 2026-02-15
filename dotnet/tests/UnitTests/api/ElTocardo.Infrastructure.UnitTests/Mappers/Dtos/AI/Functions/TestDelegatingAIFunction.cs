using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.AI;

namespace ElTocardo.Infrastructure.UnitTests.Mappers.Dtos.AI.Functions;

// Test helper: a DelegatingAIFunction subclass we can initialize in tests.
public class TestDelegatingAIFunction(AIFunction function) : DelegatingAIFunction(function);
