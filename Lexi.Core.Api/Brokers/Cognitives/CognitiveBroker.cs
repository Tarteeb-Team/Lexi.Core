//=================================
// Copyright (c) Tarteeb LLC.
// Powering True Leadership
//=================================

using System;
using System.IO;
using System.Threading.Tasks;
using Concentus.Oggfile;
using Concentus.Structs;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using Microsoft.CognitiveServices.Speech.PronunciationAssessment;
using Microsoft.VisualBasic;
using NAudio.Wave;

namespace Lexi.Core.Api.Brokers.Cognitives
{
    public class CognitiveBroker : ICognitiveBroker
    {
        string speechKey = "4c16b8cafd324366830b415ad566f667";
        string speechRegion = "centralindia";
        string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);
        string fileName = "output.ogg";
        string wavName = "output.wav";
        public async Task<string> GetOggFile(byte[] audio)
        {
            #region reg
            using (MemoryStream memoryStream = new MemoryStream())
            {
                byte[] data = audio; // Replace this with your data source

                memoryStream.Write(data, 0, data.Length);

                string folderPath = _filePath;
     

                string filePath = Path.Combine(folderPath, fileName);

                // Write the MemoryStream contents to the file
                File.WriteAllBytes(filePath, memoryStream.ToArray());

                Console.WriteLine("MemoryStream saved to file successfully.");
                ReturningConvertOggToWav(fileName, wavName);
            }
            #endregion
            return await GetJsonString();
        }


        public async Task<string> GetJsonString()
        {
            var speechConfig = SpeechConfig.FromSubscription(speechKey, speechRegion);
            speechConfig.SpeechRecognitionLanguage = "en-US";

            var pronunciationAssessmentConfig = new PronunciationAssessmentConfig(
            referenceText: "",
            gradingSystem: GradingSystem.HundredMark,
            granularity: Granularity.Phoneme,
            enableMiscue: false);
            pronunciationAssessmentConfig.EnableProsodyAssessment();
            pronunciationAssessmentConfig.EnableContentAssessmentWithTopic("greeting");

            using var audioConfig = AudioConfig.FromWavFileInput(Path.Combine(_filePath, wavName));
            using (var speechRecognizer = new SpeechRecognizer(speechConfig, audioConfig))
            {
                pronunciationAssessmentConfig.ApplyTo(speechRecognizer);
                var speechRecognitionResult = await speechRecognizer.RecognizeOnceAsync();

                // The pronunciation assessment result as a Speech SDK object
                var pronunciationAssessmentResult =
                    PronunciationAssessmentResult.FromResult(speechRecognitionResult);

                // The pronunciation assessment result as a JSON string
                var pronunciationAssessmentResultJson = speechRecognitionResult.Properties.GetProperty(PropertyId.SpeechServiceResponse_JsonResult);

                return pronunciationAssessmentResultJson;
            }
        }
        static string ReturningConvertOggToWav(string oggName, string wavName)
        {
            var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory);

            using (FileStream fileIn = new FileStream($"{filePath}{oggName}", FileMode.Open))
            using (MemoryStream pcmStream = new MemoryStream())
            {
                OpusDecoder decoder = OpusDecoder.Create(48000, 1);
                OpusOggReadStream oggIn = new OpusOggReadStream(decoder, fileIn);
                while (oggIn.HasNextPacket)
                {
                    short[] packet = oggIn.DecodeNextPacket();
                    if (packet != null)
                    {
                        for (int i = 0; i < packet.Length; i++)
                        {
                            var bytes = BitConverter.GetBytes(packet[i]);
                            pcmStream.Write(bytes, 0, bytes.Length);
                        }
                    }
                }
                pcmStream.Position = 0;
                var wavStream = new RawSourceWaveStream(pcmStream, new WaveFormat(48000, 1));
                var sampleProvider = wavStream.ToSampleProvider();
                WaveFileWriter.CreateWaveFile16($"{filePath}{wavName}", sampleProvider);
                return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, wavName);
            }
        }
    }
}
