<template>
  <ul>
    <li v-for="message of messages">{{message}}</li>
  </ul>
  <v-text-field v-model="txtMessage" />
  <v-btn @click="sendMessage">Send</v-btn>
</template>

<script setup lang="ts">
import { HttpTransportType, HubConnectionBuilder } from '@microsoft/signalr';
import { reactive, ref } from 'vue';

const messages = reactive<string[]>([]);
const txtMessage = ref('');

const connection = new HubConnectionBuilder()
  .withUrl(import.meta.env.VITE_HUB_URL, {
    skipNegotiation: true,
    transport: HttpTransportType.WebSockets,
  })
  .build();

connection.on("ReceiveMessage", (user: string, message: string) => {
  messages.push(message);
});

connection.start().catch(function (err) {
    return console.error(err.toString());
});;

function sendMessage() {
  connection.invoke("SendMessage", "user", txtMessage.value).catch(function (err) {
      return console.error(err.toString());
  });
}

</script>
