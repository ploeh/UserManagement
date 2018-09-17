{-# LANGUAGE DeriveFunctor #-}
{-# LANGUAGE LambdaCase #-}
module UserManagement where

import Data.Bifunctor
import Data.Either
import Control.Monad.Trans.Class (lift)
import Control.Monad.Trans.Except

data HttpResponse a = OK a | BadRequest String deriving (Show, Eq, Functor)

isOK :: HttpResponse a -> Bool
isOK (OK _) = True
isOK     _  = False

isBadRequest :: HttpResponse a -> Bool
isBadRequest (BadRequest _) = True
isBadRequest             _  = False

data User = User { userId :: Integer, connectedUsers :: [User] } deriving (Show, Eq)

addConnection :: User -> User -> User
addConnection (User i cns) otherUser = User i $ otherUser : cns

data UserLookupError = InvalidId | NotFound deriving (Show, Eq)

post :: Monad m =>
        (a -> m (Either UserLookupError User)) ->
        (User -> m ()) ->
        a ->
        a ->
        m (HttpResponse User)
post lookupUser updateUser userId otherUserId = do
  userRes <- first (\case
      InvalidId -> "Invalid user ID."
      NotFound  -> "User not found.")
    <$> lookupUser userId
  otherUserRes <- first (\case
      InvalidId -> "Invalid ID for other user."
      NotFound  -> "Other user not found.")
    <$> lookupUser otherUserId

  connect <- runExceptT $ do
      user <- ExceptT $ return userRes
      otherUser <- ExceptT $ return otherUserRes
      lift $ updateUser $ addConnection user otherUser
      return otherUser

  return $ either BadRequest OK connect