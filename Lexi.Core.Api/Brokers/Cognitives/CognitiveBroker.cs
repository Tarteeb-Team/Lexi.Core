//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using Concentus.Oggfile;
using Concentus.Structs;
using Lexi.Core.Api.Brokers.TelegramBroker;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.PronunciationAssessment;
using NAudio.Wave;
using System;
using System.IO;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace Lexi.Core.Api.Brokers.Cognitives
{
    public class CognitiveBroker : ICognitiveBroker
    {
        string speechKey = "4c16b8cafd324366830b415ad566f667";
        string speechRegion = "centralindia";
        private readonly ITelegramBroker telegramBroker;

        public CognitiveBroker(ITelegramBroker telegramBroker)
        {
            this.telegramBroker = telegramBroker;
        }

        public async Task<string> GetJsonString()
        {
            MemoryStream memoryStream = this.telegramBroker.ReturnFilePath();

            var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
            speechConfig.SpeechRecognitionLanguage = "en-US";

            var pronunciationAssessmentConfig = new PronunciationAssessmentConfig(
            referenceText: "",
            gradingSystem: GradingSystem.HundredMark,
            granularity: Granularity.Phoneme,
            enableMiscue: false);
            pronunciationAssessmentConfig.EnableProsodyAssessment();
            pronunciationAssessmentConfig.EnableContentAssessmentWithTopic("greeting");

            try
            {

                var format = AudioStreamFormat.GetWaveFormatPCM(
                     samplesPerSecond: 44100, bitsPerSample: 16, channels: 2);

                var callback = new AudioInputCallback(memoryStream);

                using var audioConfig = AudioConfig.FromStreamInput(callback, format);


                // Creating a SpeechRecognizer using SpeechConfig and AudioConfig
                using (var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig))
                {
                    pronunciationAssessmentConfig.ApplyTo(speechRecognizer);
                    var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();

                    // The pronunciation assessment result as a Speech SDK object
                    var pronunciationAssessmentResult =
                        PronunciationAssessmentResult.FromResult(speechRecognitionResult);

                    // The pronunciation assessment result as a JSON string
                    var pronunciationAssessmentResultJson = speechRecognitionResult
                        .Properties.GetProperty(PropertyId.SpeechServiceResponse_JsonResult);

                    return pronunciationAssessmentResultJson;
                }
            }
            catch (Exception ex)
            {

                throw;
            }
        }
        
    }

    internal class AudioInputCallback : PullAudioInputStreamCallback
    {
        private MemoryStream mStream;

        public AudioInputCallback(MemoryStream mStream)
        {
            this.mStream = mStream;
        }

        public override int Read(byte[] dataBuffer, uint size)
        {
            return this.Read(dataBuffer, 0, dataBuffer.Length);
        }

        private int Read(byte[] buffer, int offset, int count)
        {
            return mStream.Read(buffer, offset, count);
        }

        public override void Close()
        {
            mStream.Close();
            base.Close();
        }
    }
}
