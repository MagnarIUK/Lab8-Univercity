import json
import os.path
from telegram import Update
from telegram.ext import CommandHandler, CallbackContext, Application
from random import choice, randint
from math import floor
magnar = 1215850812

if not os.path.exists("tickets.json"):
    with open("tickets.json", 'w') as file:
        file.write("{}")
if not os.path.exists("config.json"):
    with open("config.json", 'w') as file:
        file.write('{"open": true}')

def get_token():
    with open("token.txt", "r") as f:
        return f.read()
async def start(update: Update, context: CallbackContext):
    user = update.effective_user.id
    with open("tickets.json", "r") as file:
        data = json.load(file)

    with open("config.json", "r") as file:
        is_open = json.load(file).get("open")
    user_ids = [u for u in data]
    if is_open:
        if str(user) in user_ids:
            ticket_number = data.get(str(user))
            text = f'{update.effective_user.first_name}, ви вже записані на розіграш.\nВаш номер квитка: {ticket_number}'
            await context.bot.send_message(chat_id=update.message.chat_id, text=text)
        else:
            ticket_number = len(data) + randint(1, magnar)
            with open("tickets.json", "w") as file:
                data[update.effective_user.id] = ticket_number
                json.dump(data, file, indent=4)
            text = f'Вітаю, {update.effective_user.first_name}, ви записалися на розіграш.\nВаш номер квитка: {ticket_number}'
            await context.bot.send_message(chat_id=update.message.chat_id, text=text, parse_mode="Markdown")
    else:
        text = f'На жаль, конкурс вже закрито('
        await context.bot.send_message(chat_id=update.message.chat_id, text=text, parse_mode="Markdown")

async def random(update: Update, context: CallbackContext):
    user = update.effective_user.id
    if user == magnar:
        with open("tickets.json", "r") as file:
            data = json.load(file)
        user_ids = [u for u in data]
        winner = data.get(choice(user_ids))

        text = f"Номер квитка переможця: {winner}"
        await context.bot.send_message(chat_id=update.message.chat_id, text=text)
async def claims(update: Update, context: CallbackContext):
    user = update.effective_user.id
    if user == magnar:
        with open("tickets.json", "r") as file:
            data = json.load(file)
        user_ids = [u for u in data]

        text = f"Всього записалося: {len(user_ids)}"
        await context.bot.send_message(chat_id=update.message.chat_id, text=text)
async def randoms(update: Update, context: CallbackContext):
    user = update.effective_user.id
    im = update.message.text.split(' ')[1]
    if user == magnar:
        try:
            with open("tickets.json", "r") as file:
                data = json.load(file)
            user_ids = [u for u in data]

            number = 0
            if im[-1] == "%":
                _im = int(im[:-1])
                number = floor((_im * len(user_ids)) / 100)
                number = max(1, number) if user_ids else 0
                number = min(len(user_ids), number)
            else:
                number = int(im)
            winners = ""
            if number <= len(user_ids):
                for i in range(0, number):
                    winner_id = choice(user_ids)
                    winners += str(data.get(winner_id)) + " "
                    user_ids.remove(winner_id)


                text = f"Номер квитка переможця(-ів): {winners}"
                await context.bot.send_message(chat_id=update.message.chat_id, text=text)
            else:
                text = f"Забагато призів."
                await context.bot.send_message(chat_id=update.message.chat_id, text=text)

        except ValueError:
            text = f"Помилка, перевірте ввід"
            await context.bot.send_message(chat_id=update.message.chat_id, text=text)

async def close(update: Update, context: CallbackContext):
    user = update.effective_user.id
    if user == magnar:
        with open("config.json", "r") as file:
            data = json.load(file)
        data["open"] = False
        with open("config.json", "w") as file:
            json.dump(data, file, indent=4)
async def openc(update: Update, context: CallbackContext):
    user = update.effective_user.id
    if user == magnar:
        with open("config.json", "r") as file:
            data = json.load(file)
        data["open"] = True
        with open("config.json", "w") as file:
            json.dump(data, file, indent=4)
def main():
    application = Application.builder().token(get_token()).build()

    start_handler = CommandHandler('start', start)
    random_handler = CommandHandler('random', random)
    randoms_handler = CommandHandler('randoms', randoms)

    application.add_handler(start_handler)
    application.add_handler(random_handler)
    application.add_handler(randoms_handler)
    application.add_handler(CommandHandler('close', close))
    application.add_handler(CommandHandler('open', openc))
    application.add_handler(CommandHandler('claims', claims))

    application.run_polling(1.0)

if __name__ == '__main__':
    main()
