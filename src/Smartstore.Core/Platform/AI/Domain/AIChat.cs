﻿#nullable enable

using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace Smartstore.Core.AI
{
    /// <summary>
    /// Represents an AI conversation consisting of a sequence of messages.
    /// </summary>
    [JsonConverter(typeof(AIChatJsonConverter))]
    public class AIChat(AIChatTopic topic)
    {
        private IDictionary<string, object?>? _metadata;
        private readonly List<AIChatMessage> _messages = [];

        public AIChatTopic Topic { get; } = topic;

        /// <summary>
        /// The name of the AI model.
        /// <c>null</c> to use the default model.
        /// </summary>
        /// <example>gpt-4o</example>
        public string? ModelName { get; set; }

        public IReadOnlyList<AIChatMessage> Messages
            => _messages;

        public bool HasMessages()
            => _messages.Count > 0;

        /// <summary>
        /// Adds messages. Empty messages are not added.
        /// </summary>
        /// <param name="messages">The messages to add.</param>
        public void AddMessages(params AIChatMessage[] messages)
        {
            if (messages != null)
            {
                _messages.AddRange(messages.Where(x => x.Content.HasValue()));
            }
        }

        /// <summary>
        /// Adds metadata using the expression of the caller.
        /// </summary>
        /// <typeparam name="T">The type of the metadata value.</typeparam>
        /// <param name="value">The metadata value.</param>
        /// <param name="expression">The expression of the caller.</param>
        /// <returns>The AIChat instance.</returns>
        public AIChat SetMetaData<T>(T value, [CallerArgumentExpression(nameof(value))] string? expression = null)
        {
            if (expression == null)
            {
                return this;
            }

            var propertyName = expression.Split('.').Last();
            Metadata[propertyName] = value;

            return this;
        }

        /// <summary>
        /// Adds metadata using the specified key and value.
        /// </summary>
        /// <param name="key">The key of the metadata.</param>
        /// <param name="value">The value of the metadata.</param>
        /// <returns>The AIChat instance.</returns>
        public AIChat SetMetaData(string key, object value)
        {
            if (string.IsNullOrEmpty(key) || value == null)
            {
                return this;
            }

            Metadata[key] = value;

            return this;
        }

        /// <summary>
        /// Gets or sets the metadata associated with the AIChat.
        /// </summary>
        public IDictionary<string, object?> Metadata
        {
            get => _metadata ??= new Dictionary<string, object?>();
            set => _metadata = value;
        }

        public override string ToString()
            => string.Join(" ", _messages.Select(x => x.ToString()));

        public string ToUserPrompt()
            => string.Join(" ", _messages.Where(x => x.Role == KnownAIMessageRoles.User).Select(x => x.ToString()));
    }
}