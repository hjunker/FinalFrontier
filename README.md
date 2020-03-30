# FinalFrontier Outlook Add-In
FinalFrontier is an Outlook Add-In to detect and prevent (spear-) phishing attacks. It is under active development by [@HolgerJunker](https://twitter.com/HolgerJunker) / https://github.com/hjunker, [@julian_basi](https://twitter.com/julian_basi) / https://github.com/JulianBaSi and https://github.com/sonnenteich

## The threat: Emotet & Co
Emotet has been the major threat for initial compromise. The attack always involves emails with malicious office documents with VBA as attachments or referred to with a link. With emotet using authentic emails for spear phishing, users are overwhelmed with recognizing such attacks.

## The solution: FinalFrontier
In my personal opinion, awareness does not work - typically companies spend a lot of time and money while the improvements are mostly limited. So... why don't we code the methods for recognizing phishy /malicious emails instead of teaching people? This is what FinalFrontier does as an Add-In for Outlook 2016.

## Detection mechanisms
FinalFrontier uses a wide and constantly growing range of checks to determine whether an email is malicious or benign:
* metadata, e.g. sender information and communication history
* links, e.g. (imho) bad TLDs, link shorteners, ...
* attachments, e.g. double extensions

More features such as machine learning, deeper attachment inspection, etc. are on the way.

## Installation
Please note that [FinalFrontier-Learner](https://github.com/hjunker/FinalFrontier-Learner) needs to be used for learning the communication history so that FinalFrontier can function properly.
Update: The learning mechanism is now also included directly in FinalFrontier.

Please note that in order to use FinalFrontier you might have to install Visual Studio Tools for Office first (https://www.microsoft.com/en-US/download/details.aspx?id=48217).

Refer to https://github.com/hjunker/FinalFrontier/blob/master/200131-FinalFrontier-Nutzerdokumentation.pdf [german]

English doc is coming soon.

## Contact, supporting my development, full-fledged & customized versions
You can send us feedback via GitHub or DM us via Twitter (see top of this page for accounts).

The greatest help is to give feedback to us. Send us your feedback, ask questions or suggest features on GitHub, let us know about additions you have made to config and/or code. THX!

## musical credits
major parts of FinalFrontier were designed and implemented while listening to the great music of Oomph! (@oomphband).
