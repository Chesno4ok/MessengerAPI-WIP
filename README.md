Documentation

Chat:

/Chat/create_chat POST

params
userId, token, chatName

body:
id array


/Chat/add_user POST

params
userId, chatId, token

body:
id array


/Chat/get_chats GET

params

userId, token

/Chat/get_chat GET

params

userId, token, chatId


Message:

/Message/send_message POST

params

userId, token, chatId, content, type (text, audio,  photo, video, .[file extension])

User:

/User/get_user GET

Param
userId

/User/get_token/ GET

Param
userId, login, password

/User/change_username POST

Param
userId, token, username

/User/check_updates GET

Param
userId, token

/User/register_user POST

Param
name, login, password

/User/search_user GET

Param
username


ToADD:
Bad registration response
